using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
//using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpreadsheetModel;
namespace NetworkController
{
    // Quick User object to send with JSON
    class User
    {
        public long ID { get; set; }
        public string name { get; set; }
    }


    /// <summary>
    /// This Network class takes form similar to the "GameController" class
    /// used for Diego and Tarik's Tankwars project in CS 3500.  This Class handles the networking
    /// and connectivity expected of The Spreadsheet when communicating with the server.  
    /// </summary>
    public static class Network
    {
        // Our event handling for connected:
        public delegate void ConnectedHandler();
        public static event ConnectedHandler Connected;

        // Event handling for Errors:
        public delegate void ConnectionErrorHandler(string err);
        public static event ConnectionErrorHandler ConnectionError;

        // Event handling for updates:
        public delegate void ServerUpdateHandler();
        public static event ServerUpdateHandler UpdateArrived;


        // Event handling for picking a SS:
        public delegate void Pick_SS(string[] Spreadsheets);
        public static event Pick_SS SpreadSheetsArrived;

        /// <summary> Reports whether or not this client has selected a SS</summary>
        public static bool SS_Chosen { get; private set; }

        private static ServerSpreadsheet spreadsheet = new ServerSpreadsheet();

        /// <summary>
        /// State representing the connection with the server
        /// </summary>
        static SocketState theServer = null;

       
        /// <summary>
        /// Our User's name
        /// </summary>
        private static string UserName;


        /// <summary>
        /// Queue to store recent issued commands by client
        /// </summary>
        public static Queue<string> commandQueue = new Queue<string>();


        /// <summary>
        /// Begins handshake.
        /// </summary>
        /// <param name="address">Address to connect to</param>
        /// <param name="userName">User name of this spreadsheet.</param>
        public static void Connect(string address, string userName)
        {
            UserName = userName;
            SS_Chosen = false;
            Networking.ConnectToServer(OnConnect, address, 1100);
        }


        /// <summary>
        /// OnConnect callback for Connect
        /// </summary>
        /// <param name="obj"></param>
        private static void OnConnect(SocketState state)
        {
            // Private bool that determines if client is connected
            if (state.ErrorOccured)
            {
                ConnectionError("Error while connecting to server");
                return;
            }

            // Grab our current state
            theServer = state;

            lock (state)
            {
                // Send the player name to the server 
                User user = new User() { ID = state.ID, name = UserName };

                string userJson = JsonConvert.SerializeObject(user);
                Networking.Send(state.TheSocket, userJson);

                // Start an event loop to receive messages from the server
                state.OnNetworkAction = OnReceive;
                Networking.GetData(state);
            }
        }


        /// <summary>
        /// OnReceive callback for getting back data
        /// </summary>
        /// <param name="obj"></param>
        private static void OnReceive(SocketState state)
        {
            if (state.ErrorOccured)
            {
                ConnectionError("Error while receiving data from server");
                return;
            }

            /* Start Editing Loop */
            UpdateLoop(state);

            // Process incoming cell edit from Server
            ProcessCellEdit(state.data.ToString());
            ProcessMessages(state);
        }


        /// <summary>
        /// Private helper method to parse and process messages sent from server.
        /// </summary>
        /// <param name="state"></param>
        private static void ProcessMessages(SocketState state)
        {
            if (state.ErrorOccured)
            {
                ConnectionError("Error while receiving data from server");
                return;
            }
            string totalData = state.GetData();


            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            // Loop until we have processed all messages.
            // We may have received more than one.
            foreach (string p in parts)
            {

                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;


                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                /*if (p[p.Length - 1] != '\n')
                    break;*/


                // Pick a spreadsheet:
                if (!SS_Chosen)
                {
                    //PickSS(totalData);
                    SpreadSheetsArrived(parts);
                    SS_Chosen = true;
                }


                // Skipping incomplete JSONS
                if (p[0] != '{' || !p.EndsWith("\n"))
                {
                    continue;
                }

                // Load and parse the incoming JSON (Cell)
                //LoadObject(p);

                // Then remove it from the SocketState's growable buffer
                state.RemoveData(0, p.Length);
            }
            state.OnNetworkAction = UpdateLoop;
            state.GetData();
        }



        /// <summary>
        /// Callback for OnReceive
        /// </summary>
        /// <param name="state"></param>
        private static void UpdateLoop(SocketState state)
        {
            if (state.ErrorOccured == true)
            {
                ConnectionError("Error while connecting to server");
                return;
            }
            string info;
            lock (state.GetData())
            {
                info = state.GetData();
            }
            string[] jobjects = Regex.Split(info, @"(?<=[\n])");
            lock (spreadsheet)
            {
                foreach (string s in jobjects)
                {
                    if (s.Length == 0)
                        continue;
                    if (s[s.Length - 1] != '\n')
                    {
                        continue;
                    }

                    JObject jObject = JObject.Parse(s);

                    JToken tokenUpdate = jObject["cellUpdate"];
                    JToken tokenSelect = jObject["cellSelected"];

                    if (tokenUpdate != null)
                    {
                        UpdateCell update = JsonConvert.DeserializeObject<UpdateCell>(s);
                        spreadsheet.EditCellToSpreadsheet(update.cellName, update.contents);

                    }
                    if (tokenSelect != null)
                    {
                        SelectCell select = JsonConvert.DeserializeObject<SelectCell>(s);

                    }
                    state.RemoveData(0, s.Length);
                }

                UpdateArrived();

            }
            state.OnNetworkAction = UpdateSpreadsheet;
            Networking.GetData(state);
        }

        /// <summary>
        /// UpdateSpreadsheet callback for updating the spreadsheet's data
        /// </summary>
        /// <param name="state"></param>
        private static void UpdateSpreadsheet(SocketState state)
        {
            if (state.ErrorOccured == true)
            {
                ConnectionError("Error while connecting to server");
                return;
            }
            string info;
            lock (state.GetData())
            {
                info = state.GetData();
            }
            string[] jobjects = Regex.Split(info, @"(?<=[\n])");
            lock (spreadsheet)
            {
                foreach (string s in jobjects)
                {
                    if (s.Length == 0)
                        continue;
                    if (s[s.Length - 1] != '\n')
                        continue;
                    JObject jObject = JObject.Parse(s);

                    JToken tokenUpdate = jObject["cellUpdate"];
                    JToken tokenSelect = jObject["cellSelected"];
                    JToken tokenDisconnect = jObject["disconnect"];
                    JToken tokenInvalid = jObject["requestError"];
                    JToken tokenShutdown = jObject["serverError"];
                    if (tokenUpdate != null)
                    {
                        UpdateCell update = JsonConvert.DeserializeObject<UpdateCell>(s);
                        spreadsheet.EditCellToSpreadsheet(update.cellName, update.contents);

                    }
                    if (tokenSelect != null)
                    {
                        SelectCell select = JsonConvert.DeserializeObject<SelectCell>(s);

                    }

                    if (tokenDisconnect != null)
                    {
                        Disconnect disconnect = JsonConvert.DeserializeObject<Disconnect>(s);
                    }
                    if (tokenInvalid != null)
                    {
                        InvalidRequest invalid = JsonConvert.DeserializeObject<InvalidRequest>(s);
                      //  Error(invalid.message);
                    }
                    if (tokenShutdown != null)
                    {
                        Shutdown shutdown = JsonConvert.DeserializeObject<Shutdown>(s);
                        //  Error(shutdown.message);
                    }

                    state.RemoveData(0, s.Length);
                }
               // ProcessInput(state);
                UpdateArrived();

            }

            Networking.GetData(state);
        }
        /// <summary>
        /// Processes incoming request from Server to update Spreadsheet
        /// from other client's input
        /// </summary>
        /// <param name="req"></param>
        private static void ProcessCellEdit(string req)
        {

        }
    }
}
