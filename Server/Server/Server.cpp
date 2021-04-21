#include "Server.h"

int Server::Init() {
	// Initialize WinSock 
	WSADATA wsaData;
	WORD ver = MAKEWORD(2, 2);
	int wsOK = WSAStartup(ver, &wsaData);
	if (wsOK != 0) {
		return wsOK;
	}

	// Create Socket
	_socket = socket(AF_INET, SOCK_STREAM, 0);
	if (_socket == INVALID_SOCKET) {
		return WSAGetLastError();
	}

	// Bind IP and Port to Socket
	sockaddr_in sock_address;
	sock_address.sin_family = AF_INET;
	sock_address.sin_port = htons(_port);
	inet_pton(AF_INET, _ip_address, &sock_address.sin_addr);

	if (bind(_socket, (sockaddr*)&sock_address, sizeof(sock_address)) == SOCKET_ERROR) {
		return WSAGetLastError();
	}

	// Tell Winsock the socket is ready for listening
	if (listen(_socket, SOMAXCONN) == SOCKET_ERROR) {
		return WSAGetLastError();
	}

	// Create the master file descriptor set and zero it;
	FD_ZERO(&_master);

	// Add Socket to FDSet to get later incoming connections
	FD_SET(_socket, &_master);

	return 0;
}


int Server::Run() {
	bool isConnected = true;
	while (isConnected) {
		fd_set copy = _master;

		int socket_count = select(0, &copy, nullptr, nullptr, nullptr);

		// Loop through all current connections
		for (int i = 0; i < socket_count; i++) {
			SOCKET current_socket = copy.fd_array[i];

			if (current_socket == _socket) {
				// Accept new connection in
				SOCKET client = accept(_socket, nullptr, nullptr);

				// Add the new connection into the list of connected clients
				FD_SET(client, &_master);

				// Client Connected Callback
				OnClientConnect(client);
			}

			else {
				const int buffer_length = 4096;
				char buffer[buffer_length];
				ZeroMemory(buffer, buffer_length);

				// Receive Message
				int bytes_in = recv(current_socket, buffer, buffer_length, 0);
				if (bytes_in <= 0) {
					// Disconnect Client using Client Disconnected callback
					OnClientDisconnect(current_socket, buffer, buffer_length);
					closesocket(current_socket);
					FD_CLR(current_socket, &_master);
				}

				else {
					// TODO: Check for JSON . . .
					OnMessageReceived(current_socket, buffer, bytes_in);
				}
			}
		}
	}

	// Remove listening socket from the master file descriptor set and close it
	FD_CLR(_socket, &_master);
	closesocket(_socket);
	WSACleanup();

	return 0;
}


// Send message to a client
void Server::SendToClient(int client_socket, const char* message, int length) {
	send(client_socket, message, length, 0);

	// TODO: SEND WITH JSON
}


void Server::BroadcastToClients(int sending_client, const char* message, int length) {
	// Send message to other clients 
	for (int i = 0; i < _master.fd_count; i++) {
		SOCKET out_socket = _master.fd_array[i];
		if (out_socket != _socket && out_socket != sending_client) {
			SendToClient(out_socket, message, length);
		}
	}
}


void Server::OnClientConnect(int client_socket) {
	std::cout << client_socket << std::endl;
	char t = 't';
	SendToClient(client_socket, &t, (int)strlen(&t));

}


void Server::OnClientDisconnect(int client_socket, const char* message, int length) {
	available_spreadsheets.erase(client_socket);
	std::cout << "Client: " << client_socket << " disconnected!" << std::endl;
}


void Server::OnMessageReceived(int client_socket, const char* message, int length) {
	JObject json = JObject::parse(message);

	// Iterate the array
	for (JObject::iterator it = json.begin(); it != json.end(); ++it) {
		if (it.key() == "name") {
			std::cout << "Client: " << it.value() << " has connected!" << '\n';

			std::string name = it.value();
			Spreadsheet* spreadsheet = new Spreadsheet(name);

			available_spreadsheets.emplace(client_socket, spreadsheet);
		}
	}
}