#include <string>
#include <exception>

/// <summary>
/// Thrown to indicate that a change to a cell will cause a circular dependency.
/// </summary>
class CircularException : public std::exception
{

};

/// <summary>
/// Thrown to indicate that a name parameter was either null or invalid.
/// </summary>
class InvalidNameException : public std::exception
{

};

