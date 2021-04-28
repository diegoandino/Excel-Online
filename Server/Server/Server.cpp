#include "Server.h"

/// <summary>
/// Initialize the server and establish means for network communication
/// </summary>
/// <returns></returns>
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

/// <summary>
/// Run the server
/// </summary>
/// <returns></returns>
int Server::Run() {
	bool is_connected = true;
	while (is_connected) {
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

/// <summary>
/// send message to client
/// </summary>
/// <param name="client_socket"> The socket (int) corresponding with that client </param>
/// <param name="message"> Message to send to the client </param>
/// <param name="length"></param>
void Server::SendToClient(int client_socket, std::string message, int length) {
	send(client_socket, message.c_str(), length, 0);
}


/// <summary>
/// Send (broadcast) changes to all clients
/// </summary>
/// <param name="sending_client"></param>
/// <param name="message"></param>
/// <param name="length"></param>
void Server::BroadcastToClients(int sending_client, std::string message, int length) {

	// Send message to other clients
	for (int i = 0; i < _master.fd_count; i++) {

		SOCKET out_socket = _master.fd_array[i];
		if (out_socket == sending_client) {

			// It should be sending Spreadsheet Cell Value Changes 
			SendToClient(out_socket, message, length);
		}
	}
}

/// <summary>
/// method called upon client connection
/// </summary>
void Server::OnClientConnect(int client_socket) {
	std::cout << client_socket << std::endl;

	// Upon connection, this client hasnt been initialized.
	isClientSetup.insert({ client_socket, false });
}

/// <summary>
/// This method is called when a client disconnects
/// </summary>
void Server::OnClientDisconnect(int client_socket, std::string message, int length) {
	lock.lock();
	std::cout << "Client: " << client_socket << " disconnected!" << std::endl;
	lock.unlock();
}

/// <summary>
/// @@@@@@@@@@@@@@@@@@@@@@@@note when to use?
/// </summary>
/// <param name="client_socket"></param>
void Server::EraseFromServer(int client_socket) {
	lock.lock();
	available_clients.erase(client_socket);
	lock.unlock();
}

/// <summary>
/// Called when a client sends a message to the server
/// If message was a regular string, we understand that the client has sent a name.
/// If message parses into a JObject we understand it as a Request. (JSON)
/// </summary>
void Server::OnMessageReceived(int client_socket, std::string message, int length)
{
 
	JObject req = JObject::parse(message);
	ProcessRequests(client_socket, message, length, req);



	std::cout << message << std::endl;

}

/// <summary>
/// Method called from OnMessageReceived.
/// When a request is sent from the client to the server here we digest that request and process it.
/// Requests range from setting up a spreadsheet, to requesting the server make a new spreadsheet to be used. 
/// </summary>
void Server::ProcessRequests(int client_socket, const std::string& message, int length, JObject req)
{

	// Setting up a client if needed
	if (!isClientSetup[client_socket]) {
		ProcessClientConnectedRequests(client_socket, message, length, req);
	}

	// Client sent a name of a nonexistent spreadsheet; create a new one.
	if (request_new_ss) {
		CreateNewSpreadsheet(client_socket, message);
		request_new_ss = false;
	}

	// Else Update Loop
	ProcessCellSelectedRequests(client_socket, message, length, req);
	ProcessCellEditedRequests(client_socket, message, length, req);
}

/// <summary>
/// This method is called from ProcessRequests.
/// Here We use an iterator to look at our req based off of the key of that request.
/// 
/// Case:
/// -	"name" : printout to console this user has connected. Send a string of available spreadsheets to client.   
/// </summary>
/// <param name="client_socket"></param>
/// <param name="message"></param>
/// <param name="length"></param>
/// <param name="req"></param>
void Server::ProcessClientConnectedRequests(int client_socket, const std::string& message, int length, JObject req)
{
	for (JObject::iterator it = req.begin(); it != req.end(); ++it)
	{
		if (it.key() == "name")
		{
			std::cout << "Client: " << it.value() << " has connected!" << '\n';

			std::string username = it.value();

			// Send Available Spreadsheets to Client
			std::string spreadsheets = get_available_spreadsheets();
			for (int i = 0; i < spreadsheets.size(); i++)
				SendToClient(client_socket, spreadsheets.c_str(), length);


			// Check if it's empty; if so send anyways, Client needs to know there's no available Spreadsheets
			// Also create a new (empty) Spreadsheet for the client
			if (spreadsheets == "") {
				SendToClient(client_socket, spreadsheets.c_str(), length);
				//request_new_ss = true;

				// Add this new spreadsheet to available spreadsheet 
				Spreadsheet* spreadsheet = new Spreadsheet();
				lock.lock();
				available_clients.emplace(client_socket, spreadsheet);
				lock.unlock();
			}
		}

		if (it.key() == "spreadsheet_name")
		{
			std::cout << "Spreadsheet Name Selected: " << it.value() << '\n';

			std::string str = it.value();

			// If the requested spreadsheet is present send it
			for (int index = 0; index < available_spreadsheets.size(); index++)
			{
				std::string temp = available_spreadsheets[index]->get_spreadsheet_name();

				if (temp == str) {
					Spreadsheet* spreadsheet = find_selected_spreadsheet(it.value());
					spreadsheet->set_spreadsheet_name(it.value());

					lock.lock();
					available_clients[client_socket] = spreadsheet;
					lock.unlock();

					isClientSetup[client_socket] = true;
					return;
				}
			}

			// Spreadsheet name was not found, it did not exists yet, create it and send it
			Spreadsheet* s = new Spreadsheet(str);
			available_spreadsheets.push_back(s);

			s->set_spreadsheet_name(str);

			lock.lock();
			available_clients[client_socket] = s;
			lock.unlock();
		}
	}

	initial_handshake_approved = true;
}


/// <summary>
/// This method is called when the client had requested a new spreadsheet.
/// </summary>
void Server::CreateNewSpreadsheet(int client_socket, std::string name) {
	Spreadsheet* s = new Spreadsheet(name);
	available_spreadsheets.push_back(s);
	available_clients[client_socket] = s;
}

/// <summary>
/// Returns a string of currently available spreadsheets separated by new lines.
/// </summary>
std::string Server::get_available_spreadsheets() {
	std::string res;
	for (int i = 0; i < available_spreadsheets.size(); i++) {
		std::string name = available_spreadsheets[i]->get_spreadsheet_name();
		res += name + "\n";
	}

	return res;
}

/// <summary>
/// Returns a spreadsheet pointer to a spreadsheet name "name"
/// Returns NULL if said spreadsheet could not be found.
/// </summary>
Spreadsheet* Server::find_selected_spreadsheet(std::string name) {
	for (std::map<int, Spreadsheet*>::iterator it = available_clients.begin(); it != available_clients.end(); ++it) {
		if (name == it->second->get_spreadsheet_name())
			return it->second;
	}

	return NULL;
}




void Server::ProcessCellSelectedRequests(int client_socket, const std::string& message, int length, JObject req) {
	// Iterate the array
	std::string cellName = "";
	for (JObject::iterator it = req.begin(); it != req.end(); ++it) {
		if (it.key() == "cellName") {
			cellName = it.value();
		}

		if (it.key() == "requestType") {
			if (it.value() == "selectCell") {
				std::cout << "Client: " << client_socket << " Has Selected Cell: " << cellName
					<< " On Spreadsheet: " << available_clients[client_socket] << '\n';

				std::string json = std::string("{" "\"" "messageType" "\"" ": " "\"" "selected"
					"\"" ", " "cellName" "\"" ": " + cellName + "\"" ", "
					"\"" "selector" "\"" ": " "\"" + std::to_string(client_socket) + "\"" "}"
				);

				BroadcastToClients(client_socket, json.c_str(), length);
			}
		}
	}
}


void Server::ProcessCellEditedRequests(int client_socket, const std::string& message, int length, JObject req) {
	// Iterate the array
	std::string content = "";
	std::string cellName = "";
	for (JObject::iterator it = req.begin(); it != req.end(); ++it) {
		if (it.key() == "contents") {
			content = it.value();
		}

		if (it.key() == "cellName") {
			cellName = it.value();
		}

		if (it.key() == "requestType") {

			// TODO :: Process and send back to client GUI to display changes
			if (it.value() == "editCell") {
				std::cout << "Client: " << client_socket << " Has Requested Edit: " << content
					<< " On Cell: " << cellName << " On Spreadsheet: " <<
					available_clients[client_socket] << '\n';

				// Store data in server Spreadsheet
				lock.lock();
				available_clients[client_socket]->set_cell_content(cellName, content);
				lock.unlock();

				// Send data over to client to display on GUI
				std::string json = std::string("{" "\"" "messageType" "\"" ": " "\"" "cellUpdated "
					"\"" ", " "cellName" "\"" ": " + cellName + "\"" ", "
					"\"" "contents" "\"" ": " "\"" + content + "\"" "}"
				);

				BroadcastToClients(client_socket, json.c_str(), length);
			}
		}
	}
}

