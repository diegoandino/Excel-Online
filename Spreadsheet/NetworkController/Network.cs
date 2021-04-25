using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NetworkController
{
    // Quick User object to send with JSON
    class User
    {
        public long ID {get; set;}
        public string name {get; set;}
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
        }


        /// <summary>
        /// Callback for OnReceive
        /// </summary>
        /// <param name="state"></param>
        private static void UpdateLoop(SocketState state)
		{
            if (state.ErrorOccured)
			{
                ConnectionError("Error on update loop");
                return;
            }

            while (true)
			{
                // Send to initial information to server
                if (commandQueue.Count >= 1)
                    Networking.Send(state.TheSocket, commandQueue.Dequeue());
            }
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
