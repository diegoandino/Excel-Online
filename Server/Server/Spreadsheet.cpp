#include "Spreadsheet.h"


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

/// <summary>
/// default constructor
/// </summary>
Spreadsheet::Spreadsheet() : spreadsheet_name("spwedsweet UwU"), changed(false)
{
}


Spreadsheet::Spreadsheet(std::string& name) : spreadsheet_name(name), changed(false)
{
}

//std::map<Cell, std::string> Spreadsheet::get_spreadsheet_contents() {
//	std::map<Cell, std::string> content; 
//
//	return content; 
//}

std::string Spreadsheet::get_spreadsheet_name() { return spreadsheet_name;  }

// MODIFIED PROTECTION FOR PS5
/// <summary>
/// The contents of the named cell becomes text.  The method returns a
/// list consisting of name plus the names of all other cells whose value depends, 
/// directly or indirectly, on the named cell. The order of the list should be any
/// order such that if cells are re-evaluated in that order, their dependencies 
/// are satisfied by the time they are evaluated.
/// 
/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
/// list {A1, B1, C1} is returned.
/// </summary>
std::list<std::string> Spreadsheet::set_cell_content(std::string& cellName, double number)
{
	std::string num = std::to_string(number);
	set_generic_content(cellName, num);

	//updates the dependencies
	std::vector<std::string> s;
	cell_graph.replace_dependents(cellName, s);

	std::list<std::string> list = get_cells_to_recalculate(cellName);

	for (std::string s : list)
	{
		std::string new_cont(update_value(s));
		cells_map[s].set_cell_content(new_cont);
	}

	return list;
}

/// <summary>
/// The contents of the named cell becomes text.  The method returns a
/// list consisting of name plus the names of all other cells whose value depends, 
/// directly or indirectly, on the named cell. The order of the list should be any
/// order such that if cells are re-evaluated in that order, their dependencies 
/// are satisfied by the time they are evaluated.
/// 
/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
/// list {A1, B1, C1} is returned.
/// </summary>
std::list<std::string> Spreadsheet::set_cell_content(std::string& cellName, std::string text)
{
	//updates the dependencies
	std::vector<std::string> vec;
	cell_graph.replace_dependents(cellName, vec);

	std::list<std::string> list = get_cells_to_recalculate(cellName);

	if (text.empty() && cells_map.contains(cellName))
	{
		cells_map.erase(cellName);
	}
	else if (!text.empty())
	{
		set_generic_content(cellName, text);

		for (auto i : list)
		{
			if (i == cellName)
				continue;

			std::string s("");
			s = i; //THERE IS AN ERROR HERE
			std::cout << i << std::endl;
			std::cout << s << std::endl;
			std::string new_cont(update_value(s));
			cells_map[s].set_cell_content(new_cont); // this is updating the current cell twice
		}
	}

	return list;
}

/// <summary>
/// If content is null, throws an ArgumentNullException.
/// 
/// Otherwise, if name is null or invalid, throws an InvalidNameException.
/// 
/// Otherwise, if content parses as a double, the contents of the named
/// cell becomes that double.
/// 
/// Otherwise, if content begins with the character '=', an attempt is made
/// to parse the remainder of content into a Formula f using the Formula
/// constructor.  There are then three possibilities:
/// 
///   (1) If the remainder of content cannot be parsed into a Formula, a 
///       SpreadsheetUtilities.FormulaFormatException is thrown.
///       
///   (2) Otherwise, if changing the contents of the named cell to be f
///       would cause a circular dependency, a CircularException is thrown,
///       and no change is made to the spreadsheet.
///       
///   (3) Otherwise, the contents of the named cell becomes f.
/// 
/// Otherwise, the contents of the named cell becomes content.
/// 
/// If an exception is not thrown, the method returns a list consisting of
/// name plus the names of all other cells whose value depends, directly
/// or indirectly, on the named cell. The order of the list should be any
/// order such that if cells are re-evaluated in that order, their dependencies 
/// are satisfied by the time they are evaluated.
/// 
/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
/// list {A1, B1, C1} is returned.
/// </summary>
std::list<std::string> Spreadsheet::set_contents_of_cell(std::string& name, std::string& content)
{
	if (content.empty())
		throw std::invalid_argument("NO EMPTY STRINGS");

	std::string normalized = normalize(name);
	name_check(normalized);

	changed = true;
	stack_changes.push(normalized);

	//three cases possible content is either a:
	//number
	try
	{
		double d = std::stod(normalized);
		return set_cell_content(normalized, d);
	}
	catch (std::invalid_argument e)
	{
		//formula
		if (content.size() > 0 && content.at(0) == '=')
		{
			std::string s = content.substr(1);
			Formula formula(s);

			return set_cell_content(normalized, formula);
		}
		//text
		else
		{
			return set_cell_content(normalized, content);
		}
	}
}

/// <summary>
/// undoes the last change to the spreadsheet 
/// </summary>
/// <returns>the new content of the cell</returns>
std::string Spreadsheet::undo(std::string& name)
{
	name = stack_changes.peek();
	name = normalize(name);
	return revert(stack_changes.pop(), true);
}

/// <summary>
/// Reverts a specific cell back to its previous value
/// </summary>
/// <param name="name">name of the cell to be reverted</param>
/// <returns>the new content of the cell</returns>
std::string Spreadsheet::revert(const std::string& name, bool is_undo)
{
	try
	{
		if (is_undo)
		{
			return cells_map.at(name).revert();
		}
		else
		{	
			//should add to the changes stack
			return cells_map.at(name).revert();
		}
	}
	catch (std::out_of_range e)
	{
		std::cout << "revert on nonexistent cell" << std::endl;
		return "";
	}
}

/// <summary>
/// If changing the contents of the named cell to be the formula would cause a 
/// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
/// 
/// Otherwise, the contents of the named cell becomes formula. The method returns a
/// list consisting of name plus the names of all other cells whose value depends,
/// directly or indirectly, on the named cell. The order of the list should be any
/// order such that if cells are re-evaluated in that order, their dependencies 
/// are satisfied by the time they are evaluated.
/// 
/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
/// list {A1, B1, C1} is returned.
/// </summary>
std::list<std::string> Spreadsheet::set_cell_content(std::string& cellName, Formula formula)
{
	//adds the variables to the dependency graph
	std::vector<std::string> variables = formula.get_variables();
	cell_graph.replace_dependents(cellName, variables);

	//checks if the graph is valid
	try
	{
		std::list<std::string> list = get_cells_to_recalculate(cellName);

		//an "=" at the begining of a string is how we determine if something is a formula
		std::string f = "=" + formula.to_string();
		set_generic_content(cellName, f);//Fs in the chat bois

		for (std::string s : list)
		{
			std::string new_cont(update_value(s));
			cells_map[s].set_cell_content(new_cont);
		}

		return list;
	}
	catch (CircularException& e)
	{
		for (std::string s : variables)
			cell_graph.remove_dependency(cellName, s);

		throw CircularException("HOLY SHIT A WILD CIRCULAR DEPENDENCY APPEARED");
	}

	return std::list<std::string>();
}


void Spreadsheet::set_spreadsheet_name(const std::string& new_name) {
	spreadsheet_name = new_name;
}

/// <summary>
/// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
/// value should be either a string, a double, or a Formula.
/// </summary>
std::string Spreadsheet::get_cell_contents(const std::string& name)
{
	if (cells_map.contains(name))
	{
		std::string s(cells_map[name].get_cell_content());
		return s;
	}
	else
	{
		return "";
	}
}

/// <summary>
/// Enumerates the names of all the non-empty cells in the spreadsheet.
/// </summary>
std::vector<std::string> Spreadsheet::get_nonempty_cells()
{
	std::vector<std::string> ret;

	for (auto s : cells_map)
	{
		ret.push_back(s.first);
	}

	return ret;
}


/// <summary>
/// helper method for the set methods
/// adds an item if there is not one or sets it if there is one already
/// </summary>
/// <param name="name">name of the cell</param>
/// <param name="content">content to be set</param>
void Spreadsheet::set_generic_content(std::string& name, std::string& content)
{
	std::string s(name);


	if (cells_map.contains(name))
	{
		cells_map[name].set_cell_content(content);
	}
	else
	{
		Cell c(name, content);
		cells_map.insert({ name, c });//after this line the cell loses its name
	}
}

/// <summary>
/// A convenience method for invoking the other version of GetCellsToRecalculate
/// with a singleton set of names.  See the other version for details.
/// </summary>
std::list<std::string> Spreadsheet::get_cells_to_recalculate(const std::string& name)
{
	std::vector<std::string> vec;
	vec.push_back(name);

	return get_cells_to_recalculate(vec);
}

/// <summary>
/// Requires that names be non-null.  Also requires that if names contains s,
/// then s must be a valid non-null cell name.
/// 
/// If any of the named cells are involved in a circular dependency,
/// throws a CircularException.
/// 
/// Otherwise, returns an enumeration of the names of all cells whose values must
/// be recalculated, assuming that the contents of each cell named in names has changed.
/// The names are enumerated in the order in which the calculations should be done.  
/// 
/// For example, suppose that 
/// A1 contains 5
/// B1 contains 7
/// C1 contains the formula A1 + B1
/// D1 contains the formula A1 * C1
/// E1 contains 15
/// 
/// If A1 and B1 have changed, then A1, B1, and C1, and D1 must be recalculated,
/// and they must be recalculated in either the order A1,B1,C1,D1 or B1,A1,C1,D1.
/// The method will produce one of those enumerations.
/// 
/// Please note that this method depends on the abstract GetDirectDependents.
/// It won't work until GetDirectDependents is implemented correctly.
/// </summary>
std::list<std::string> Spreadsheet::get_cells_to_recalculate(std::vector<std::string>& names)
{
	std::list<std::string> changed;
	std::unordered_set<std::string> visited;

	for (std::string name : names)
	{
		if (!visited.contains(name))
		{
			visit(name, name, visited, changed);
		}
	}

	return changed;
}

/// <summary>
/// Returns an enumeration, without duplicates, of the names of all cells whose
/// values depend directly on the value of the named cell.  In other words, returns
/// an enumeration, without duplicates, of the names of all cells that contain
/// formulas containing name.
/// 
/// For example, suppose that
/// A1 contains 3
/// B1 contains the formula A1 * A1
/// C1 contains the formula B1 + A1
/// D1 contains the formula B1 - C1
/// The direct dependents of A1 are B1 and C1
/// </summary>
std::vector<std::string> Spreadsheet::get_direct_dependents(std::string& name)
{
	return cell_graph.get_dependees(name);
}

/// <summary>
/// A helper for the GetCellsToRecalculate method.
/// </summary>
void Spreadsheet::visit(const std::string& start, std::string& name, std::unordered_set<std::string>& visited, std::list<std::string>& changed)
{
	visited.insert(name);
	for (std::string n : get_direct_dependents(name))
	{
		if (n == start)
		{
			throw CircularException("HOLY SHIT IT'S A CIRCULAR DEPENDENCY");
		}
		else if (!visited.contains(n))
		{
			visit(start, n, visited, changed);
		}
	}

	changed.push_front(name);
}

/// <summary>
/// calculates the value of the cell
/// should acount for the return type (double, string, Formula)
/// </summary>
/// <param name="name"></param>
/// <returns>the value of the calculated cell</returns>
std::string Spreadsheet::update_value(std::string& name)
{
	std::string s(get_cell_contents(name));
	Cell cell(name, s);

	if (cell.is_formula())
	{
		std::string s(cell.get_cell_content());
		Formula f(s);

		std::string evaluate(f.evaluate().get_content());
		//Cell c(name, evaluate);
		return f.evaluate().get_content();
	}
	else
	{
		return s;
	}
}

/// <summary>
/// Normalizes the string 
/// this means converting it to lowercase
/// </summary>
/// <param name="s"></param>
/// <returns>a normalized string</returns>
std::string Spreadsheet::normalize(const std::string& s)
{
	std::locale loc;
	std::string str(s);

	for (std::string::size_type i = 0; i < s.length(); ++i)
		str[i] = std::tolower(s[i], loc);

	return str;
}

/// <summary>
/// checks a string to see if it is a variable
/// </summary>
/// <param name="s"></param>
/// <returns>true if the string is a variable</returns>
bool Spreadsheet::name_check(const std::string& s)
{
	std::string varPattern = "[a-zA-Z]?[0-9]*";

	return std::regex_match(s, std::regex(varPattern));
}