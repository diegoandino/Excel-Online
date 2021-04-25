#include "cell.h"

Cell::Cell() : cell_name(""), cell_value()
{
}

Cell::Cell(std::string name, CellValue value) : cell_name(name), cell_value(value)
{
}

std::string Cell::get_cell_name()
{
	return cell_name;
}

std::string Cell::get_cell_value()
{
	return cell_value.get_value();
}

bool Cell::is_empty()
{
	return cell_value.is_empty();
}