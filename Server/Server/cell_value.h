#pragma once

#include "Formula.h"

/// <summary>
/// cell value can be things:
/// string
/// formula
/// formula error
/// </summary>
template<typename T>
class CellValue {

public:
	CellValue();

	CellValue(T& cont);

	T get_content();

	void set_content(T& cont);

	bool is_empty();

private:
	
	T& content;
	
};