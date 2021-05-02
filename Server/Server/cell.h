#pragma once

#include <string>
#include <sstream>
#include <map>  

#include "cell_value.h"
#include "changes_stack.h"

class Cell {
public:
	Cell();
	//Cell(std::string name, CellValue value);
	Cell(const std::string& name, std::string& value);
	Cell(const Cell& c);
	//~Cell();

	std::string get_cell_name(); 
	std::string get_cell_content(); 
	void set_cell_content(std::string& s);
	
	bool is_empty(); 
	bool is_formula();

	std::string revert();

private:
	std::string cell_name; 
	//CellValue cell_value; 
	changes_stack stack;
};