#include "cell.h"

Cell::Cell()
{
	cell_name = "";
	cell_value = CellValue();
}

Cell::Cell(std::string name, CellValue value) : cell_name(name), cell_value(value)
{
}

std::string Cell::get_cell_name()
{
	return cell_name;
}

std::string Cell::get_cell_content()
{
	return cell_value.get_content();
}

bool Cell::is_empty()
{
	return cell_value.is_empty();
}