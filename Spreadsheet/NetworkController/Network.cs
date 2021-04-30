using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using SS;
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
        public delegate void ServerUpdateHandler(Dictionary<string, string> dictionary);
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

        private static int id;
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
        private static Spreadsheet serverSpreadsheet = new Spreadsheet((s) => Regex.IsMatch(s, @"^[a-zA-Z]*[0-9]+$"), (s) => s.ToUpper(), "ps6");


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
            //theServer = state;

            lock (state)
            {
                Connected();

                Networking.Send(state.TheSocket, UserName);
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

            lock (state)
            {
                ProcessMessages(state);
                UpdateLoop(state);
            }

            Networking.GetData(state);
        }


        /// <summary>
        /// Private helper method process User's spreadsheet selection
        /// </summary>
        /// <param name="state"></param>
        private static void ProcessMessages(SocketState state)
        {
            lock (server)
            {
                if (state.ErrorOccured)
                {
                    ConnectionError("Error while receiving data from server");
                    return;
                }
                string totalData = state.GetData();
                string[] parts = Regex.Split(totalData, @"\n+");

                SpreadSheetsArrived(parts);
                state.RemoveData(0, totalData.Length);
            }
        }
    

        /// <summary>
        /// Callback for OnReceive
        /// </summary>
        /// <param name="state"></param>
        private static void UpdateLoop(SocketState state)
        {
            if (state.ErrorOccured)
            {
                ConnectionError("Error while receiving data from server");
                return;
            }

            lock (server)
            {
                if (server.ErrorOccured)
                {
                    ConnectionError("Error on update loop");
                    return;
                }

                if (spreadsheetNameQueue.Count >= 1)
                    Networking.Send(server.TheSocket, spreadsheetNameQueue.Dequeue());

                string totalData = state.GetData();
                try
                {
                    JObject json = JObject.Parse(totalData);
                    if (json.ContainsKey("cellName"))
                        if (json.ContainsKey("contents"))
                            serverSpreadsheet.SetContentsOfCell(json["cellName"].ToString(), json["contents"].ToString());
                }

                catch (Exception e) {}

                state.RemoveData(0, totalData.Length);
                state.OnNetworkAction = UpdateLoop;
                server.GetData();
            }
        }
    }
}
