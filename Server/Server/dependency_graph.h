#pragma once

#include <unordered_map>
#include <unordered_set>
#include <string>
#include <list>

class DependencyGraph {

public:
	DependencyGraph();
	int get_size();
	bool has_dependents(std::string s);
	bool has_dependees(std::string s);
	std::list<std::string> get_dependents(std::string s);
	std::list<std::string> get_dependees(std::string s);

	void add_dependency(std::string s, std::string t);
	void remove_dependency(std::string s, std::string t);
	void replace_dependents(std::string s, std::list<std::string> new_dependents);


private:
	int size;

	std::unordered_map<std::string, std::unordered_set<std::string>> dependents;
	std::unordered_map<std::string, std::unordered_set<std::string>> dependees;
};