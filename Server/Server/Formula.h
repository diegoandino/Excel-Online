#pragma once

#include <string>
#include <list>
#include <stack>
#include <iostream>
#include <vector>
#include <algorithm> 
#include <cctype>
#include <locale>
#include <regex>
#include "cell_value.h"

class Formula
{

public:
	Formula(std::string& formula);

	bool is_valid(std::string& str);

	double lookup(std::string& str);

	CellValue evaluate();

	std::list<std::string> get_variables();

	bool equals(Formula& other);

	std::string to_string();

private:

	std::vector<std::string> get_tokens(std::string& formula);

	bool token_is_valid(std::string& token);

	bool op_par_follow_rule(std::string& s);

	bool is_operator(std::string& s);

	bool perform_div_mult(std::stack<double> values, std::stack<std::string> operators, double num);

	bool perform_add_subs(std::stack<double> values, std::stack<std::string> operators);

	bool is_variable(std::string& s);

	bool operator_is_on_top(std::stack<std::string>, std::string tkn);

	std::vector<std::string> tokens;

	//string operations
	//these methods were static in SO
	inline void ltrim(std::string& s);
	inline void rtrim(std::string& s);
	inline void trim(std::string& s);

};

