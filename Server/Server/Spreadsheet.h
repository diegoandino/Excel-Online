#pragma once

#include <string>
#include <sstream>
#include <map>  

#include "cell.h"
#include "dependency_graph.h"

class Spreadsheet {
public:
	Spreadsheet() {}
	Spreadsheet(std::string& name) : spreadsheet_name(name) {}
	~Spreadsheet() {}

	std::map<Cell, std::string> get_spreadsheet_contents(); 

	std::string get_spreadsheet_name(); 

	void set_cell_content(std::string cellName, std::string content); 

private:
	std::map<std::string, Cell> cells; 
	DependencyGraph deGraph; 

	std::string spreadsheet_name;
	bool changed; 
};