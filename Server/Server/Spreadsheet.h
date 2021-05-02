#pragma once

#include <string>
#include <sstream>
#include <map> 
#include <unordered_map>
#include <vector>
#include <regex>


#include "cell.h"
#include "dependency_graph.h"
#include "Formula.h"
#include "changes_stack.h"

class Spreadsheet {
public:
	Spreadsheet();
	Spreadsheet(std::string& name);

	//std::map<Cell, std::string> get_spreadsheet_contents(); 

	std::string get_spreadsheet_name(); 

	void set_spreadsheet_name(const std::string& name); 

	std::string get_cell_contents(const std::string& name);
	std::vector<std::string> get_nonempty_cells();

	std::list<std::string> set_contents_of_cell(std::string& name, std::string& content);

	std::string undo();
	std::string revert(const std::string& name);

private:

	void set_generic_content(std::string& name, std::string& content);
	std::vector<std::string> get_direct_dependents(std::string& name);

	std::list<std::string> get_cells_to_recalculate(std::vector<std::string>& names);
	std::list<std::string> get_cells_to_recalculate(const std::string& name);

	void visit(const std::string& start, std::string& name, std::unordered_set<std::string>& visited, std::list<std::string>& changed);

	std::list<std::string> set_cell_content(std::string& cellName, double number);
	std::list<std::string> set_cell_content(std::string& cellName, std::string text);
	std::list<std::string> set_cell_content(std::string& cellName, Formula formula);

	std::string update_value(std::string& name);

	std::string normalize(const std::string& s);
	bool name_check(const std::string& s);

	//instance variables
	std::unordered_map<std::string, Cell> cells_map;
	DependencyGraph cell_graph;

	std::string spreadsheet_name;
	bool changed; 

	/// <summary>
	/// stores the names of changed cells
	/// </summary>
	changes_stack stack_changes;
};

class CircularException : public std::exception
{
	std::string _msg;
public:
	CircularException(const std::string& msg) : _msg(msg) {}

	virtual const char* what() const noexcept override
	{
		return _msg.c_str();
	}
};