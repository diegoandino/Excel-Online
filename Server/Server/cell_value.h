#pragma once

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
	/*~CellValue();*/

	std::string get_content();

	void set_content(std::string& content);

	void set_error();

	bool is_empty();

	bool is_error();

private:
	std::string content; 
};