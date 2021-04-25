#include "cell.h"
template<typename T>
Cell<T>::Cell() : cell_name(""), cell_value()
{
}

template<typename T>
Cell<T>::Cell(std::string name, CellValue<T> value) : cell_name(name), cell_value(value)
{
}

template<typename T>
std::string Cell<T>::get_cell_name()
{
	return cell_name;
}

template<typename T>
T Cell<T>::get_cell_value()
{
	return cell_value.get_value();
}

template<typename T>
bool Cell<T>::is_empty()
{
	return cell_value.is_empty();
}


