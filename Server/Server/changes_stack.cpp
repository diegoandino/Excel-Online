#include "changes_stack.h"

/// <summary>
/// creates a stack of strings to store changes
/// </summary>
/// <param name="max_size_">the max possible size of the stack</param>
changes_stack::changes_stack(int max_size_) :max_size(max_size_)
{
}

/// <summary>
/// deletes and returns the topmost item
/// returns an empty string if the stack is empty
/// </summary>
/// <returns>topmost item</returns>
std::string changes_stack::pop()
{
	if (change_stack.size() > 0)
	{
		std::string ret = change_stack.front();
		change_stack.pop_front();
		return ret;
	}
	else
		return "";
}

/// <summary>
/// returns the topmost item, does not delete it
/// </summary>
/// <returns>topmost item</returns>
std::string changes_stack::peek()
{
	if (change_stack.size() > 0)
		return change_stack.front();
	else
		return "";
}

/// <summary>
/// pushes and element to the top of the stack
/// </summary>
/// <param name="s"></param>
void changes_stack::push(std::string& s)
{
	change_stack.push_front(s);

	if (change_stack.size() > max_size)
	{
		change_stack.pop_back();
	}
}

/// <summary>
/// empties the stack
/// </summary>
/// <returns></returns>
void changes_stack::clear()
{
	change_stack.clear();
}

/// <summary>
/// checks if the stack is empty
/// </summary>
/// <returns>true if the stack is empty</returns>
bool changes_stack::is_empty()
{
	return change_stack.empty();
}

/// <summary>
/// the size of the stack
/// </summary>
/// <returns>size of the stack</returns>
int changes_stack::size()
{
	return change_stack.size();
}