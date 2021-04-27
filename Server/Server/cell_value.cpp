#include "cell_value.h"

CellValue::CellValue()
{
}

CellValue::CellValue(std::string& cont)
{
	content = cont;
}

std::string CellValue::get_content()
{
	return ""; 
}

std::string CellValue::get_value() {
	return "";
}

void CellValue::set_content(std::string& cont)
{
	
}

bool CellValue::is_empty()
{
	return false; 
}