using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Threading; 

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
        /// Available Spreadsheets to send
        /// </summary>
        public static Queue<string> spreadsheetNameQueue = new Queue<string>();


        /// <summary>
        /// Access to the Server SocketState
        /// </summary>
        public static SocketState server; 


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

                Connected();

                // Send the player name to the server 
                /* User user = new User() { ID = state.ID, name = UserName };

                 string userJson = JsonConvert.SerializeObject(user);
                 Networking.Send(state.TheSocket, userJson);*/

                Networking.Send(state.TheSocket, UserName + "\n");
                server = state;

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

            //Thread t = new Thread(UpdateLoop);
            lock (state)
			{
                ProcessMessages(state);

                /* Start Editing Loop */
                UpdateLoop();
                //t.Start();
            }

            Networking.GetData(server);
        }


        /// <summary>
        /// Private helper method to parse and process messages sent from server.
        /// </summary>
        /// <param name="state"></param>
        private static void ProcessMessages(SocketState state)
        {
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
        }


        /// <summary>
        /// Callback for OnReceive
        /// </summary>
        /// <param name="state"></param>
        private static void UpdateLoop()
        {
            lock (server)
			{
                if (server.ErrorOccured)
                {
                    ConnectionError("Error on update loop");
                    return;
                }

                if (spreadsheetNameQueue.Count >= 1)
                    Networking.Send(server.TheSocket, spreadsheetNameQueue.Dequeue());
            }
        }
    }
}
