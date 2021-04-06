// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SpreadsheetUtilities
{
    /*
     * Diego Andino
     * September 19th, 2020
     */

    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {

        private string formula;
        private string normalizedFormula;
        private bool isNormalized; 
        private List<string> variables = new List<string>(); 
        
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
            this.formula = formula; 
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            this.formula = formula;
            isNormalized = false;

            // Base check for valid formula
            if (IsValidFormula(formula))
            {
                // Normalize if valid
                string res = normalize(normalizedFormula);
                isNormalized = true;

                // Add to variables list
                AddNormalizedVariablesToList(res, variables);

                // Check for input validity
                if (isValid(res) == false)
                    throw new FormulaFormatException("Formula is not valid due to input validator.");
            }

            else
                throw new FormulaFormatException("Formula is not valid.");
        }

        /// <summary>
        /// Private helper method to determine if given base formula is valid.
        /// </summary>
        /// <param name="formula">Formula input</param>
        /// <returns>True if valid, else false</returns>
        
        private bool IsValidFormula(string formula)
        { 
            List<string> tokens = new List<string>(GetTokens(formula));

            // For balanced parenthesis rule
            int startParenthesisCount = 0;
            int endParenthesisCount = 0;

            if (tokens.Count < 1)
                return false;

            // Starting token rule; must be a number, a variable, or an opening parenthesis.
            if (!Regex.IsMatch(tokens[0].Trim(), @"^[(]|[a-zA-Z_](?:[a-zA-Z_]|\d)*|(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?"))
                return false;

            // Ending token rule; must be a number, a variable, or an closing parenthesis.
            if (!Regex.IsMatch(tokens[tokens.Count - 1].Trim(), @"^[)]|[a-zA-Z_](?:[a-zA-Z_]|\d)*|(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                return false;

            for (int i = 0; i < tokens.Count; i++)
            {
                bool validToken = false;

                // Check if current item contains (, ), +, -, *, /, variables, and decimal real numbers
                if (Regex.IsMatch(tokens[i].Trim(), @"^[()]|[+\-*\/]|[a-zA-Z_](?:[a-zA-Z_]|\d)*|(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                    validToken = true;

                if (validToken == true)
                {
                    // Check if it's a scientific notation number to parse it correctly for the normalized formula
                    if (Regex.IsMatch(tokens[i].Trim(), @"(?:\d+|\d*\.\d+)[?:[eE][\+-]?[\d+?]+"))
                    {
                        double.TryParse(tokens[i].Trim(), out double res);
                        normalizedFormula += res.ToString(); 
                    }

                    else
                        normalizedFormula += tokens[i].Trim();

                    // Check if it's not a number
                    if (!double.TryParse(tokens[i].Trim(), out double parsedDouble))
                    {
                        bool isVariable = Regex.IsMatch(tokens[i].Trim(), @"[a-zA-Z_](?:[a-zA-Z_]|\d)*");

                        if (isVariable)
                           variables.Add(tokens[i].Trim());
                    }

                    // Parenthesis/Operator Following Rule
                    if (Regex.IsMatch(tokens[i].Trim(), @"[+\-*\/]|[(]"))
                    {
                        // Prevents Out of Bounds Exception
                        if (i + 2 > tokens.Count)
                            continue;

                        // Must be either a number, a variable, or an opening parenthesis.
                        if (!Regex.IsMatch(tokens[i + 1].Trim(), @"^[(]|[a-zA-Z_](?:[a-zA-Z_]|\d)*|(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                            return false;
                    }

                    // Increment start parenthesis count to meet rule
                    if (tokens[i] == "(")
                        startParenthesisCount++;

                    // Right Parentheses Rule
                    else if (tokens[i] == ")")
                    {
                        endParenthesisCount++;

                        if (endParenthesisCount > startParenthesisCount)
                            return false;
                    }

                    // Extra Following Rule
                    if (Regex.IsMatch(tokens[i].Trim(), @"^[)]|[a-zA-Z_](?:[a-zA-Z_]|\d)*|(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"))
                    {
                        if (i + 2 > tokens.Count)
                            continue;

                        else if (!Regex.IsMatch(tokens[i + 1].Trim(), @"[+\-*\/]|[)]"))
                            return false;
                    }
                }
            }

            return true; 
        }

        /// <summary>
        /// Private method to add variable to variables list.
        /// </summary>
        private void AddNormalizedVariablesToList(string formula, List<string> list)
        {
            if (isNormalized)
            {
                list.Clear();

                string[] variables = Regex.Matches(formula, @"[a-zA-Z_](?:[a-zA-Z_]|\d)*").Cast<Match>().Select(c => c.Value).ToArray();
                
                foreach (string s in variables)
                    list.Add(s);
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            if (lookup == null)
                return new FormulaError("Lookup delegate cannot be null");
                
            Stack<string> operatorStack = new Stack<string>();
            Stack<double> valueStack = new Stack<double>();

            List<string> substrings = new List<string>(GetTokens(formula));

            foreach (string s in substrings)
            {
                // Check if the current string is a double
                if (double.TryParse(s.Trim(), out double parsedDouble))
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        if (parsedDouble == 0)
                            return new FormulaError("Cannot divide by zero.");

                        double value = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, parsedDouble, value));
                    }

                    else
                        valueStack.Push(parsedDouble);
                }

                // Check if the current string is a variable using Regex
                else if (Regex.IsMatch(s.Trim(), @"[a-zA-Z_](?:[a-zA-Z_]|\d)*"))
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        double value = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, lookup(s.Trim()), value));
                    }

                    else
                        valueStack.Push(lookup(s.Trim()));
                }

                else if (s == "+" || s == "-")
                {
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        double value1 = valueStack.Pop();
                        double value2 = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, value1, value2));
                    }

                    operatorStack.Push(s);
                }

                else if (s == "*" || s == "/")
                    operatorStack.Push(s);

                else if (s == "(")
                    operatorStack.Push(s);

                else if (s == ")")
                {
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        double value1 = valueStack.Pop();
                        double value2 = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, value1, value2));
                    }

                    if (operatorStack.IsOnTop("("))
                        operatorStack.Pop();

                    // Check if next operator is * or /
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        double value1 = valueStack.Pop();
                        double value2 = valueStack.Pop();
                        string _operator = operatorStack.Pop();

                        valueStack.Push(ApplyOperator(_operator, value1, value2));
                    }
                }
            }

            // Check if operator stack is empty
            if (operatorStack.Count < 1 && valueStack.Count == 1)
                return valueStack.Pop();

            if (operatorStack.Count == 1 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
            {
                if (valueStack.Count == 2)
                {
                    double value1 = valueStack.Pop();
                    double value2 = valueStack.Pop();
                    string _operator = operatorStack.Pop();

                    return ApplyOperator(_operator, value1, value2);
                }
            }

            return new FormulaError("Invalid Expression. Check valid variables, parenthesis or operators.");
        }


        /// <summary>
        /// Private method that provides the Evaluate method a way to apply
        /// _operators to the values popped from the Stack.
        /// </summary>
        /// <param name="_operator"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns>The value of the operation</returns>
        private static double ApplyOperator(string _operator, double val2, double val1)
        {
            switch (_operator)
            {
                case "+":
                    return val1 + val2;

                case "-":
                    return val1 - val2;

                case "*":
                    return val1 * val2;

                case "/":
                    return val1 / val2;
            }

            return 0;
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return normalizedFormula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Formula) || obj is null)
                return false;

            return (obj as Formula).normalizedFormula == normalizedFormula;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null))
                return ReferenceEquals(f2, null); 

            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return normalizedFormula.GetHashCode(); 
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason) : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }


    /// <summary>
    /// Static class that represents an extension for the Stack class
    /// </summary>
    static class StackExtensions
    {
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
}
