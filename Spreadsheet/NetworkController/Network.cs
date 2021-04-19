using System;
using System.Collections.Generic;
using System.Text;
using NetworkUtil;
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
   public class Network
    {
        // Our event handling for connected:
        public delegate void ConnectedHandler();
        public event ConnectedHandler Connected;

        // Event handling for Errors:
        public delegate void ConnectionErrorHandler(string err);
        public event ConnectionErrorHandler ConnectionError;

        // Event handling for updates:
        public delegate void ServerUpdateHandler();
        public event ServerUpdateHandler UpdateArrived;

        /// <summary>
        /// State representing the connection with the server
        /// </summary>
        SocketState theServer = null;

        /// <summary>
        /// Our User's name
        /// </summary>
        private string UserName;

        // Default constructor
        public Network()
        {

        }


        /// <summary>
        /// Begins handshake.
        /// </summary>
        /// <param name="address">Address to connect to</param>
        /// <param name="userName">User name of this spreadsheet.</param>
        public void Connect(string address, string userName)
        {
            UserName = userName;
            Networking.ConnectToServer(OnConnect, address, 1100);
        }

        
        /// <summary>
        /// OnConnect callback for Connect
        /// </summary>
        /// <param name="obj"></param>
        private void OnConnect(SocketState state)
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
        private void OnReceive(SocketState state)
		{
            if (state.ErrorOccured)
			{
                ConnectionError("Error while receiving data from server");
                return; 
			}
		}


        /// <summary>
        /// Sends selected cell data to server
        /// </summary>
        /// <param name="state"></param>
        private void OnCellSelected(SocketState state)
		{
            if (state.ErrorOccured)
            {
                ConnectionError("Error while sending cell data to server");
                return;
            }

            // Grab our current state
            /*theServer = state;

            lock (state)
			{
                Networking.Send(state.TheSocket, cell);
			}*/
        }
    }
}
