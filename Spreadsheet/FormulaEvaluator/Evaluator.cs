using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /**
     * Diego Andino
     * September 4, 2020
     */

    /// <summary>
    /// Static class that represents an Infix notation evaluator for strings.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Delegate used to lookup a variable used in the string
        /// so it can be used as an integer in the evaluation.
        /// </summary>
        /// <param name="v">Value to look up</param>
        /// <returns>The value of the variable as an integer</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// Method that Evaluates an expression using standard Infix
        /// notation. 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>The result of the evaluation as in integer.</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            if (string.IsNullOrWhiteSpace(exp))
                throw new ArgumentException($"Argument, '{nameof(exp)}', cannot be null, empty, or whitespace.");

            Stack<string> operatorStack = new Stack<string>();
            Stack<int> valueStack = new Stack<int>(); 

            string[] substrings = Regex.Split(exp.Trim(), "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            foreach (string s in substrings)
            {
                //Check for whitespace
                if (s == " ")
                    continue;

                bool isInteger = int.TryParse(s.Trim(), out int parsedInt);
                bool isVariable = Regex.IsMatch(s.Trim(), "^[A-Za-z]+[0-9]+$"); 

                // Check if the current string is an integer
                if (isInteger)
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        if (valueStack.Count < 1)
                            throw new ArgumentException("Value stack cannot be empty.");

                        int value = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, parsedInt, value));
                    }

                    else
                    {
                        valueStack.Push(parsedInt);
                        continue;
                    }
                }

                // Check if the current string is a variable using Regex
                else if (isVariable)
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        if (valueStack.Count < 1)
                            throw new ArgumentException("Value stack cannot be empty.");

                        int value = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, variableEvaluator(s.Trim()), value));
                    }

                    else
                    {
                        valueStack.Push(variableEvaluator(s.Trim()));
                        continue;
                    }
                }
                
                // Check if the current string is either a + or -
                else if (s == "+" || s == "-")
                {
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        if (valueStack.Count < 2)
                            throw new ArgumentException("Value stack must contain more than one element.");

                        int value1 = valueStack.Pop();
                        int value2 = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, value1, value2));
                    }

                    operatorStack.Push(s);
                    continue;
                }

                // Check if the current string is either * or /
                else if (s == "*" || s == "/")
                    operatorStack.Push(s);
                
                // Check if the current string starts a parenthesis
                else if (s == "(")
                    operatorStack.Push(s);

                // Check for end parenthesis
                else if (s == ")")
                {
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        if (valueStack.Count < 2)
                            throw new ArgumentException("Value stack must contain more than one element.");

                        int value1 = valueStack.Pop();
                        int value2 = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, value1, value2));
                    }

                    // Pop the ")" from the operator stack
                    if (operatorStack.IsOnTop("("))
                        operatorStack.Pop();
                    else
                        throw new ArgumentException("Could not find start parenthesis.");

                    // Check if next operator is * or /
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        if (valueStack.Count < 2)
                            throw new ArgumentException("Value stack must contain more than one element.");

                        int value1 = valueStack.Pop();
                        int value2 = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, value1, value2));
                    }
                }
            }

            // Check if operator stack is empty
            if (operatorStack.Count < 1 && valueStack.Count == 1)
                return valueStack.Pop();

            // Check if operator stack is not empty
            if (operatorStack.Count == 1 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
            {
                if (valueStack.Count == 2)
                {
                    int value1 = valueStack.Pop();
                    int value2 = valueStack.Pop();
                    string _operator = operatorStack.Pop();

                    return ApplyOperator(_operator, value1, value2);
                }

                throw new ArgumentException("Couldn't find the second value for the sum or substraction.");
            }

            throw new ArgumentException("Invalid Expression. Check valid variables, parenthesis or operators.");
        }

        /// <summary>
        /// Private method that provides the Evaluate method a way to apply
        /// _operators to the values popped from the Stack.
        /// </summary>
        /// <param name="_operator"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns>The value of the operation</returns>
        private static int ApplyOperator(string _operator, int val2, int val1)
        {
            switch(_operator)
            {
                case "+":
                    return val1 + val2;

                case "-":
                    return val1 - val2;

                case "*":
                    return val1 * val2;

                case "/":
                    if (val2 == 0)
                        throw new ArgumentException("Cannot divide by zero.");
                    return val1 / val2;
            }

            throw new ArgumentException("Could not complete operation.");
        }
    }
}

/// <summary>
/// Static class that represents an extension for the Stack class
/// </summary>
static class StackExtensions
{
    public static int i = 0;
    /// <summary>
    /// Extension that checks whether the stack is empty and
    /// if stack.Peek() returns the correct string.
    /// </summary>
    /// <param name="stack">Stack passed in</param>
    /// <param name="s">String to be checked for</param>
    /// <returns>Boolean value</returns>
    public static bool IsOnTop(this Stack<string> stack, string s)
    {
        if (stack.Count < 1)
            return false;
        return stack.Peek() == s;
    }
}