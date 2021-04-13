#include <iostream>

#include "Server.h"

int main() {
	Server server("127.0.0.1", 1100);

	if (server.Init() != 0)
		return -1; 

	server.Run();
}