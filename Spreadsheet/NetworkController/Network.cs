using System;
using System.Collections.Generic;
using System.Text;
using NetworkUtil;
namespace NetworkController
{

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
        public delegate void ErrorHandler(string err);
        public event ErrorHandler Error;

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
            Networking.ConnectToServer(OnConnect, address, 11000);
        }

        /// <summary>
        /// OnConnect callback for Connect
        /// </summary>
        /// <param name="obj"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccured)
            {
                Error("Error while connecting to server");
                return;
            }

            // Grab our current state
            theServer = state;

           /* lock (state)
            {
                // Send the player name to the server          
                Networking.Send(state.TheSocket, PlayerName + "\n");

                // Start an event loop to receive messages from the server
                state.OnNetworkAction = ReceiveMessage;
                Networking.GetData(state);
            }*/
        }
    }
}
