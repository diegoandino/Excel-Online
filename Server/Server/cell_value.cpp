#include "cell_value.h"

CellValue::CellValue() : content("")
{
}

CellValue::CellValue(std::string& cont)
{
	if (cont == "FORMULA::ERROR")
		set_error();	
	else
		content = cont;
}

std::string CellValue::get_content()
{
	return content; 
}

void CellValue::set_content(std::string& cont)
{
	content = cont;
}

void CellValue::set_error()
{
	content = "FORMULA::ERROR";
}

bool CellValue::is_empty()
{
	return content == ""; 
}

bool CellValue::is_error()
{
	return content == "FORMULA::ERROR";
}
