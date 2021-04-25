#pragma once

#include "Formula.h"
#include <string>

/// <summary>
/// cell value can be things:
/// string
/// formula
/// formula error
/// </summary>

class CellValue {
public:
	CellValue();
	CellValue(std::string& cont);

	std::string get_value();

	std::string get_content();

	void set_content(std::string& content);

	bool is_empty();

private:
	std::string content; 
};