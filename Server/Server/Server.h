#pragma once
#pragma execution_character_set("utf-8")

#ifdef _WIN32
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0A00 /* Windows 10+ */
#endif
#include <WS2tcpip.h>
#include <winsock2.h>

#else // else, it's *nix sockets
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <unistd.h>
#include <errno.h>
#endif

#include <iostream>
#include <stdlib.h>
#include <string>
#include <sstream>
#include <stdio.h>
#include <map>
#include <fstream>
#include <thread>
#include <mutex>
#include <nlohmann/json.hpp>
#include <exception>
#include <iostream>

#include "Spreadsheet.h"
#include "changes_stack.h"

// For convenience
using JObject = nlohmann::json;

#pragma comment (lib, "ws2_32.lib")

#if !defined(_WIN32)
#define SOCKET int
#endif

class Server {
public :
	Server(const char* ip_address, int port) : _ip_address(ip_address), _port(port) {}

	int Init();	// Initialize TCPListener
	int Run();	// Run the Listener

protected:
	void OnClientConnect(int client_socket);										// Handler for when client connect
	void OnClientDisconnect(int client_socket, const char* message, int length);										// Handler for when client disconnects
	void OnMessageReceived(int client_socket, const char* message, int length);		// Handler for when a message is received

	void SendToClient(int client_socket,  const char* message, int length);			// Handler to send data to clients
	void BroadcastToClients(int sending_client, const char* message, int length);	// Handler to broadcast data to clients

private:
	const char* _ip_address;								// IP Address that the server will run on  
	int			_port;										// Port number for server
	int			_socket;									// Socket for listening
	int			_max_socket;								// Keep track of the max. socket in the fd_set
	fd_set		_master;									// Master file descriptor set
	bool		request_new_ss;								// When we are trying to make a new spreadsheet

	std::map<int, Spreadsheet*> available_clients;			// Maps clients to spreadsheets
	std::mutex lock;										// Mutex for available_spreadsheets
	std::map<int, int> isClientSetup;						// maps each Client and reports whether or not their initial setup is done.

	std::string get_available_spreadsheets();
	
	std::vector<Spreadsheet*> available_spreadsheets;

	std::map<Spreadsheet*, std::vector<int>> sp_to_client;

	std::map<Spreadsheet*, std::map<std::string, changes_stack>> cell_changes;  //changes for specific cells

	Spreadsheet* find_selected_spreadsheet(std::string name);

	void EraseFromServer						(int client_socket);
	void ProcessCellSelectedRequests			(int client_socket, const char* message, int length, JObject req);
	void ProcessCellEditedRequests				(int client_socket, const char* message, int length, JObject req);
	void ProcessRequests						(int client_socket, const char* message, int length, JObject req);
	void ProcessUndoRequests					(int client_socket, JObject req);
	void ProcessRevertRequests					(int client_socket, JObject req);
	void CreateNewSpreadsheet					(int client_socket, std::string name);
	void ProcessClientUsername					(int client_socket, const char* message, int length);
	void ProcessClientFilename					(int client_socket, const char* message);

	std::string normalize(const std::string& s);

	bool initial_handshake_approved = false;
};