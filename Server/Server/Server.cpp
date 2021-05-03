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
void Server::SendToClient(int client_socket, const char* message, int length) {
	send(client_socket, message, length, 0);
}


/// <summary>
/// Send (broadcast) changes to all clients
/// </summary>
/// <param name="sending_client"></param>
/// <param name="message"></param>
/// <param name="length"></param>
void Server::BroadcastToClients(int sending_client, const char* message, int length) {
	// Send message to other clients
	/*for (std::map<int, Spreadsheet*>::iterator it = available_clients.begin(); it != available_clients.end(); ++it) {
		if(available_clients[sending_client] = available_clients[it->first])
			SendToClient(it->first, message, length);
	}*/

	Spreadsheet* sp = available_clients[sending_client];
	for (int i : sp_to_client[sp])
	{
		SendToClient(i, message, length);
	}
}


/// <summary>
/// method called upon client connection
/// </summary>
void Server::OnClientConnect(int client_socket) {
	// Upon connection, this client hasnt been initialized.
	isClientSetup.insert({ client_socket, false });
}


/// <summary>
/// This method is called when a client disconnects
/// </summary>
void Server::OnClientDisconnect(int client_socket, const char* message, int length) {
	lock.lock();
	std::cout << "Client: " << client_socket << " disconnected!" << std::endl;

	isClientSetup.erase(client_socket);

	Spreadsheet* sp = available_clients[client_socket];
	//clears available_clients of the disconnected client
	available_clients.erase(client_socket);

	//clears sp_to_client of the disconnected client
	for (int i =  0; i < sp_to_client[sp].size(); i++ ) 
	{
		if (i == client_socket)
		{
			sp_to_client[sp].erase(sp_to_client[sp].begin() + i - 1);
			break;
		}
	}
	
	lock.unlock();
}


/// <summary>
/// </summary>
/// <param name="client_socket"></param>
void Server::EraseFromServer(int client_socket) {
	lock.lock();
	available_clients.erase(client_socket);
	lock.unlock();
}


/// <summary>
/// Called when a client sends a message to the server
/// If message parses into a JObject we understand it as a Request. (JSON)
/// </summary>
void Server::OnMessageReceived(int client_socket, const char* message, int length)
{
	try
	{
		JObject req = JObject::parse(message);
		ProcessRequests(client_socket, message, length, req);
	}
	catch (std::exception e)
	{
		ProcessRequests(client_socket, message, length, NULL); //or "\0"
	}
}


/// <summary>
/// Method called from OnMessageReceived.
/// When a request is sent from the client to the server here we digest that request and process it.
/// Requests range from setting up a spreadsheet, to requesting the server make a new spreadsheet to be used. 
/// </summary>
void Server::ProcessRequests(int client_socket, const char* message, int length, JObject req)
{
	//if the client is sending their username
	if (isClientSetup[client_socket] == 0)
	{
		std::string string(message);
		string = string.substr(0,string.length()-1);
		ProcessClientUsername(client_socket, string.c_str(), length);
	}

	//or if the client sent a spreadsheet name
	else if (isClientSetup[client_socket] == 1)
	{
		ProcessClientFilename(client_socket, message);
	}

	else
	{
		// Else Update Loop
		ProcessUndoRequests(client_socket, message, length, req);
		ProcessCellSelectedRequests(client_socket, message, length, req);
		ProcessCellEditedRequests(client_socket, message, length, req);
	}
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
void Server::ProcessClientConnectedRequests(int client_socket, const char* message, int length, JObject req)
{
	for (JObject::iterator it = req.begin(); it != req.end(); ++it)
	{
		if (it.key() == "name")
		{
			std::cout << "Client: " << it.value() << " has connected!" << '\n';

			std::string username = it.value();

			// Send Available Spreadsheets to Client
			std::string spreadsheets = get_available_spreadsheets();
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
					sp_to_client[spreadsheet].push_back(client_socket);
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
			sp_to_client[s].push_back(client_socket);
			lock.unlock();
		}
	}

	initial_handshake_approved = true;
}


/// <summary>
/// Processes the client's username request and sends a list of spreadsheets back
/// </summary>
/// <param name="client_socket">connected socket</param>
/// <param name="message">username</param>
/// <param name="length"></param>
void Server::ProcessClientUsername(int client_socket, const char* message, int length)
{
	std::cout << "Client: " << message << " has connected!" << "\n";

	std::string username = message;

	// Send Available Spreadsheets to Client
	std::string spreadsheets = get_available_spreadsheets();
	SendToClient(client_socket, spreadsheets.c_str(), spreadsheets.size());

	// Check if it's empty; if so send anyways, Client needs to know there's no available Spreadsheets
	// Also create a new (empty) Spreadsheet for the client
	if (spreadsheets == "")
	{
		SendToClient(client_socket, spreadsheets.c_str(), length);

		// Add this new spreadsheet to available spreadsheet 
		Spreadsheet* spreadsheet = new Spreadsheet();
		lock.lock();
		available_clients.emplace(client_socket, spreadsheet);
		sp_to_client[spreadsheet].push_back(client_socket);
		lock.unlock();
	}

	isClientSetup[client_socket] = 1;
}

/// <summary>
/// Processes the client's filename request
/// </summary>
/// <param name="client_socket">connected socket</param>
/// <param name="message">filename</param>
void Server::ProcessClientFilename(int client_socket, const char* message)
{
	std::cout << "Spreadsheet filename Selected: " << message << '\n';

	//advances the client in the handshake
	isClientSetup[client_socket] = 2;

	std::string str = message;

	// If the requested spreadsheet is present send it
	for (int index = 0; index < available_spreadsheets.size(); index++)
	{
		std::string temp = available_spreadsheets[index]->get_spreadsheet_name();

		if (temp == str)
		{
			Spreadsheet* spreadsheet = find_selected_spreadsheet(message);
			spreadsheet->set_spreadsheet_name(message);
			
			lock.lock();
			available_clients[client_socket] = spreadsheet;
			sp_to_client[spreadsheet].push_back(client_socket);

			// Send all existing cells to this client.
			for (std::string cellName : spreadsheet->get_nonempty_cells())
			{
				std::cout << "open cell: " << cellName << std::endl;

				std::string json = std::string("{" "\"" "messageType" "\"" ": " "\"" "cellUpdated"
					"\"" ", "  "\""  "cellName" "\"" ": " "\"" + cellName + "\"" ", "
					"\"" "contents" "\"" ": " "\"" + spreadsheet->get_cell_contents(cellName) + "\"" "}" + "\n");

				SendToClient(client_socket, json.c_str(), json.size());
			}
			
			std::string id(std::to_string(client_socket) + "\n");
			SendToClient(client_socket, id.c_str(), id.size());

			lock.unlock();

			return;
		}
	}

	// Spreadsheet name was not found, it did not exists yet, create it and send it
	Spreadsheet* s = new Spreadsheet(str);
	available_spreadsheets.push_back(s);

	s->set_spreadsheet_name(str);

	lock.lock();
	available_clients[client_socket] = s;
	sp_to_client[s].push_back(client_socket);
	lock.unlock();

	initial_handshake_approved = true;
}


void Server::ProcessCellSelectedRequests(int client_socket, const char* message, int length, JObject req) {
	// Iterate the array
	std::string cellName = "";
	for (JObject::iterator it = req.begin(); it != req.end(); ++it) {
		if (it.key() == "cellName") {
			cellName = it.value();
		}

		if (it.key() == "requestType") {
			if (it.value() == "selectCell") {
				std::cout << "Client: " << client_socket << " Has Selected Cell: " << cellName
					<< " On Spreadsheet: " << available_clients[client_socket]->get_spreadsheet_name() << '\n';

				std::string json("{" "\"" "messageType" "\"" ": " "\"" "cellSelected"
					"\"" ", "  "\""  "cellName" "\"" ": " "\"" + cellName + "\"" ", "
					"\"" "selector" "\"" ": " "\"" + std::to_string(client_socket) + "\"" "}\n"
				);

				BroadcastToClients(client_socket, json.c_str(), json.size());
			}
		}
	}
}


void Server::ProcessCellEditedRequests(int client_socket, const char* message, int length, JObject req) {
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
					available_clients[client_socket]->get_spreadsheet_name() << '\n';

				// Store data in server Spreadsheet
				lock.lock();

				try {
					available_clients[client_socket]->set_contents_of_cell(cellName, content);
				}
				catch (InvalidRequestError e)
				{
					// Send invalid Request error to client.
					std::string json = std::string("{" "\"" "messageType" "\"" ": " "\"" "requestError"
						"\"" ", " "\"" "cellName" "\"" ": " "\"" + cellName + "\"" ", "
						"\"" "message" "\"" ": " "\"" + e.what() + "\"" "}" + "\n"
					);

					std::cout << "JSON BEING SENT: " << json << std::endl;
					SendToClient(client_socket, json.c_str(), json.size());
					lock.unlock();
					return;
					
				}
				lock.unlock();

				// Send data over to client to display on GUI
				std::string json = std::string("{" "\"" "messageType" "\"" ": " "\"" "cellUpdated "
					"\"" ", " "\"" "cellName" "\"" ": " "\"" + cellName + "\"" ", "
					"\"" "contents" "\"" ": " "\"" + content + "\"" "}" + "\n"
				);

				std::cout << "JSON BEING SENT: " << json << std::endl;

				BroadcastToClients(client_socket, json.c_str(), json.size());
			}
		}
	}
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
	if (available_spreadsheets.size() == 0) {
		return "\n\n";
	}
	for (int i = 0; i < available_spreadsheets.size(); i++) {
		std::string name = available_spreadsheets[i]->get_spreadsheet_name();
		res += name + "\n";
	}
	res += "\n";
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

/// <summary>
/// Processes an undo request on a spreadsheet
/// </summary>
/// <param name="client_socket"></param>
void Server::ProcessUndoRequests(int client_socket, const char* message, int length, JObject req)
{
	//get the client's spreadsheet
	//undo the last command
	//broadcast to clients

	for (JObject::iterator it = req.begin(); it != req.end(); ++it)
	{
		if (it.key() == "requestType")
		{
			if (it.value() == "undo")
			{
				lock.lock();
				Spreadsheet* sp = available_clients[client_socket];
				std::string name("");
				std::string content(sp->undo(name));
				lock.unlock();

				// Send data over to client to display on GUI
				std::string json = std::string("{" "\"" "messageType" "\"" ": " "\"" "cellUpdated "
					"\"" ", " "\"" "cellName" "\"" ": " "\"" + name + "\"" ", "
					"\"" "contents" "\"" ": " "\"" + content + "\"" "}" + "\n"
				);

				BroadcastToClients(client_socket, json.c_str(), json.size());
			}
		}
	}

}
