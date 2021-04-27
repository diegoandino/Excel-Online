#include "dependency_graph.h"
#include <stdexcept> // for exeptions

//this is mainly a translation from my own code in 3500

DependencyGraph::DependencyGraph()
{
	size = 0;
}

int DependencyGraph::get_size()
{
	return size;
}

/// <summary>
/// checks if a key has dependents, returns false if the key does not exist
/// </summary>
/// <param name="s">string to search for</param>
/// <returns>if a key has dependents</returns>
bool DependencyGraph::has_dependents(std::string& s)
{
	try
	{
		std::unordered_set<std::string> set = dependents.at(s);
		return set.size() > 0;
	}
	catch (std::out_of_range& e)
	{
		return false;
	}

}


/// <summary>
/// checks if a key has dependees, returns false if the key does not exist
/// </summary>
/// <param name="s">string to search for</param>
/// <returns>if a key has dependees</returns>
bool DependencyGraph::has_dependees(std::string& s)
{
	try
	{
		std::unordered_set<std::string> set = dependees.at(s);
		return set.size() > 0;
	}
	catch (std::out_of_range& e)
	{
		return false;
	}
}


/// <summary>
/// Enumerates dependents(s).
/// </summary>
std::vector<std::string> DependencyGraph::get_dependents(std::string& s)
{
	std::vector<std::string> ret;

	if (has_dependents(s))
	{
		for (std::string str : dependents.at(s))
		{
			ret.push_back(str);
		}
	}

	return ret;
}


std::vector<std::string> DependencyGraph::get_dependees(std::string& s)
{
	std::vector<std::string> ret;

	if (has_dependees(s))
	{
		for (std::string str : dependees.at(s))
		{
			ret.push_back(str);
		}
	}

	return ret;
}

/// <summary>
/// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
/// 
/// <para>This should be thought of as:</para>   
/// 
///   t depends on s
///
/// </summary>
/// <param name="s"> s must be evaluated first. T depends on S</param>
/// <param name="t"> t cannot be evaluated until s is</param>        /// 
void DependencyGraph::add_dependency(std::string& s, std::string& t)
{
	bool ee = false;
	bool ent = false;

	//first takes care of the dependent side of things
	if (dependents.contains(s))
	{
		int ent_size = dependents.size();
		dependents.at(s).insert(t);

		//ent = ent_size != dependents.size();
		ent = true;
	}
	else
	{
		//creates a new HashSet for the dependents of s and adds t to it
		std::unordered_set<std::string> s_dependents;
		s_dependents.insert(t);
		//creates a new key, s, and maps it to the HashSet containing t
		dependents.insert({ s, s_dependents }); //don't ask me about this syntax, C++ is wild
		ent = true;
	}

	//second takes care of the dependee side of things
	if (dependees.contains(t))
	{
		int ee_size = dependees.size();
		dependees.at(t).insert(s);

		//ee = dependees.at(t).size() != ee_size;
		ee = true;
	}
	else
	{
		std::unordered_set<std::string> t_dependees;
		t_dependees.insert(s);
		dependees.insert({ t, t_dependees });
		ee = true;
	}

	//increments the size only if something new was added
	if (ent && ee)
		size++;
}

/// <summary>
/// Removes the ordered pair (s,t), if it exists
/// </summary>
/// <param name="s"></param>
/// <param name="t"></param>
void DependencyGraph::remove_dependency(std::string& s, std::string& t)
{

	if (dependents.contains(s) && dependees.contains(t))
	{		
		dependents.erase(t);
		dependees.erase(s);

		if (dependents.contains(s) && dependents.at(s).size() == 0)
		{
			dependents.erase(s);
		}

		if (dependees.contains(t) && dependees.at(t).size() == 0)
		{
			dependees.erase(t);
		}
		
		size--;

	}

	//it was originally here
	//size--;
}

void DependencyGraph::replace_dependents(std::string& s, std::vector<std::string>& new_dependents)
{
	//removes all the dependcies related to s' dependents
	for (std::string str : get_dependents(s))
	{
		remove_dependency(s, str);
	}

	//adds all new nodes from newDependents as dependencies
	for(std::string str : new_dependents)
	{
		add_dependency(s, str);
	}
}

/// <summary>
/// The size of dependees(s).
/// This property is an example of an indexer.  If dg is a DependencyGraph, you would
/// invoke it like this:
/// dg["a"]
/// It should return the size of dependees("a")
/// </summary>
int DependencyGraph::operator[](std::string s)
{
	return get_dependees(s).size();
}
