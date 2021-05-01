#include "cell.h"

Cell::Cell()
{
	cell_name = "";
	cell_value = CellValue();
}

Cell::Cell(std::string name, CellValue value) : cell_name(name), cell_value(value)
{
}

Cell::Cell(std::string& name, std::string& value) : cell_name(name)
{
	CellValue cv(value);
	cell_value = cv;
}

std::string Cell::get_cell_name()
{
	return cell_name;
}

std::string Cell::get_cell_content()
{
	return cell_value.get_content();
}

void Cell::set_cell_content(std::string& s)
{
	cell_value = s;
}

bool Cell::is_empty()
{
	return cell_value.is_empty();
}

bool Cell::is_formula()
{
	return cell_value.is_formula();
}

