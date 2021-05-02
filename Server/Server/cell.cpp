#include "cell.h"

Cell::Cell()
{
	cell_name = "";
}

//Cell::Cell(std::string name, CellValue value) : cell_name(name)
//{
//}

Cell::Cell(const std::string& name, std::string& value) : cell_name(name)
{
	stack.push(value);
}

Cell::Cell(const Cell& c)
{
	cell_name = c.cell_name;
	stack = c.stack;
}

std::string Cell::get_cell_name()
{
	return cell_name;
}

std::string Cell::get_cell_content()
{
	return stack.peek();
}

void Cell::set_cell_content(std::string& s)
{
	stack.push(s);
}

bool Cell::is_empty()
{
	return stack.is_empty();
}

/// <summary>
/// checks if the content is a formula
/// </summary>
/// <returns>true if the current content is a formula</returns>
bool Cell::is_formula()
{
	return !stack.is_empty() && stack.peek().length() > 0 && stack.peek()[0] == '=';
}

/// <summary>
/// reverts the cell to its previous stage
/// 
/// if the cell is "hey", and then it is changed to "hello" and this method is called,
/// then the cell will be set to "hey" and a string containing "hey"
/// is going to be returned
/// </summary>
/// <returns>the contents of the cell after being reverted</returns>
std::string Cell::revert()
{
	stack.pop(); //pops the current content
	return stack.peek();
}
