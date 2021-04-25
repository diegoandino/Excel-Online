#pragma once

#include <iostream>
#include <stdlib.h>
#include <string>
#include <sstream>
#include <WS2tcpip.h>
#include <winsock2.h>
#include <stdio.h>
#include <map>
#include <fstream>
#include <thread>
#include <mutex>
#include <nlohmann/json.hpp>

#include "Spreadsheet.h"

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
	void OnClientDisconnect(int client_socket, const char* message, int length);										// Handler for when client disconnects
	void OnMessageReceived(int client_socket, const char* message, int length);		// Handler for when a message is received

	void SendToClient(int client_socket, const char* message, int length);			// Handler to send data to clients
	void BroadcastToClients(int sending_client, const char* message, int length);	// Handler to broadcast data to clients

private:
	const char* _ip_address;								// IP Address that the server will run on  
	int			_port;										// Port number for server
	int			_socket;									// Socket for listening
	fd_set		_master;										// Master file descriptor set

	std::map<int, Spreadsheet*> available_spreadsheets;		// Returns the available spreadsheets in the server
	std::mutex spreadsheet_lock;								// Mutex for available_spreadsheets

	void EraseFromServer				(int client_socket);
	void ProcessClientConnectedRequests	(int client_socket, const char* message, int length, JObject req);
	void ProcessCellSelectedRequests	(int client_socket, const char* message, int length, JObject req);
	void ProcessCellEditedRequests		(int client_socket, const char* message, int length, JObject req);
	void ProcessRequests				(int client_socket, const char* message, int length, JObject req);
};