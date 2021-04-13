#pragma once

#include <iostream>
#include <string>
#include <sstream>
#include <WS2tcpip.h>
#include <winsock2.h>
#include <stdio.h>
#include <fstream>

#include <nlohmann/json.hpp>
// For convenience
using JObject = nlohmann::json;

#pragma comment (lib, "ws2_32.lib")

class Server {
public :
	Server(const char* ip_address, int port) : _ip_address(ip_address), _port(port) {}

	int Init();	// Initialize TCPListener
	int Run();	// Run the Listener

protected:
	void OnClientConnect(int client_socket);										// Handler for when client connect
	void OnClientDisconnect(int client_socket);										// Handler for when client disconnects
	void OnMessageReceived(int client_socket, const char* message, int length);		// Handler for when a message is received

	void SendToClient(int client_socket, const char* message, int length);			// Handler to send data to clients
	void BroadcastToClients(int sending_client, const char* message, int length);	// Handler to broadcast data to clients

private:
	const char* _ip_address;			// IP Address that the server will run on  
	int			_port;					// Port number for server
	int			_socket;				// Socket for listening
	fd_set		_master;				// Master file descriptor set
};