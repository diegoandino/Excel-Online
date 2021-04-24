#pragma once

#include <string>
#include <sstream>
#include <map>  

#include "cell_value.h"

template<typename T>
class Cell {
public :
	Cell();
	Cell(std::string name, CellValue<T> value);

	std::string get_cell_name(); 
	T get_cell_value(); 
	
	bool is_empty(); 

private:
	std::string cell_name; 
	CellValue cell_value; 
};