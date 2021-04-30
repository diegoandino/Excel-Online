#pragma once

#include <string>
#include <sstream>
#include <map>  

#include "cell_value.h"

class Cell {
public:
	Cell();
	Cell(std::string name, CellValue value);
	//~Cell();

	std::string get_cell_name(); 
	std::string get_cell_content(); 
	void set_cell_content(std::string& s);
	
	bool is_empty(); 

private:
	std::string cell_name; 
	CellValue cell_value; 
};