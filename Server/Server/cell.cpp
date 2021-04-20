#include "cell.h"

std::string Cell::get_cell_name()
{
	return cell_name;
}

std::string Cell::get_cell_value()
{
	return std::string();
}

bool Cell::is_empty()
{
	return &value == NULL;
}


