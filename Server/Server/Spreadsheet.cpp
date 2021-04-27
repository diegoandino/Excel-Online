#include "Spreadsheet.h"
#include "cell.h"

/// <summary>
/// This Class represents the beginning internals of our Spreadsheet program.
/// It utilizes a Dependency graph to keep track of cell dependencies, as well
/// as using a Formula class to evaluate formulas.
/// @Author Tarik Vu u0759288 
/// 9/25/20
///  
/// EDIT: As of 9/29/20 we are on the PS5 Branch! - Tarik Vu
/// 
/// Edit: 4/21/2021 Modifed for C++ for 3505 final.
/// @Author "It Should work now"
/// </summary>

std::map<Cell, std::string> Spreadsheet::get_spreadsheet_contents() {
	std::map<Cell, std::string> content; 

	return content; 
}

std::string Spreadsheet::get_spreadsheet_name() { return spreadsheet_name;  }

void Spreadsheet::set_cell_content(std::string cellName, std::string content) {
	cells[cellName] = Cell(cellName, CellValue(content));
}