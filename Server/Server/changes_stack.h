#pragma once

#include <string>
#include <list>

class changes_stack
{

public:
	changes_stack(int max_size_ = 100);
	std::string pop();
	std::string peek();
	void push(std::string& s);
	void clear();
	bool is_empty();
	int size();

private:
	//elements will be added to the front of the list
	//older elements will be towards the back
	//newers elements will be towards the front
	std::list<std::string> change_stack;
	int max_size;

};