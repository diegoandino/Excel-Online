#pragma once

#include <string>
#include <list>
#include <stack>
#include <iostream>
#include <vector>
#include "cell_value.h"

class Formula
{

public:
	Formula(std::string& formula);

	bool is_valid(std::string& str);

	bool look_up(std::string& str, double num);

	CellValue Evaluate();

	std::list<std::string> get_variables();

	//bool equals() TODO

	//TODO operator overload



private:

	std::vector<std::string> get_tokens(std::string& formula);

	bool token_is_valid(std::string& formula);

	bool op_par_follow_rule(std::string& s);

	bool is_operator(std::string& s);

	bool perform_div_mult(std::stack<double> values, std::stack<double> operators, double num);

	bool perform_add_subs(std::stack<double> values, std::stack<double> operators);

	bool is_variable(std::string& s);

	std::vector<std::string> tokens;
};

