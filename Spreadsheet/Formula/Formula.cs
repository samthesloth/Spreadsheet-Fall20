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
//
// Author: Sam Peters

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using ExtensionMethods;

namespace SpreadsheetUtilities
{
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
        //Holds tokens of the formula
        private string[] tokens;

        private string validVar = "^[a-zA-Z_]+[0-9a-zA-Z_]*$";

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
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
            //Creates tokens from formula
            tokens = GetTokens(formula).ToArray();

            //Assign normalize and/or isValid if null
            if (normalize == null)
                normalize = s => s;
            if (isValid == null)
                isValid = s => true;

            //Check if string was empty with no tokens
            if (tokens.Length == 0)
                throw new FormulaFormatException("No tokens found in formula. Check formula input.");

            //Check if first string is valid
            double throwaway;
            if (!Double.TryParse(tokens[0], out throwaway) && !Double.TryParse(tokens[0], System.Globalization.NumberStyles.Float, null, out throwaway) && !Regex.IsMatch(tokens[0], validVar) && !tokens[0].Equals("("))
                throw new FormulaFormatException("The first token of the given string is invalid. Check formula input.");

            //Goes through tokens, ensuring all are valid and normalizing necessary variables
            int openParentheses = 0;
            if (tokens[0].Equals("("))
                openParentheses++;
            int closeParentheses = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                //Check if invalid token
                string current = tokens[i];
                if (!Double.TryParse(current, out throwaway) && !Double.TryParse(current, System.Globalization.NumberStyles.Float, null, out throwaway) && !Regex.IsMatch(current, validVar) && !current.Equals("(") && !current.Equals(")") && !isOperator(current))
                {
                    throw new FormulaFormatException("Invalid token given. Check token at index " + i + " in the formula input.");
                }

                //Checks if var, then normalizes and reassign var, then check if valid
                if (Regex.IsMatch(current, validVar))
                {
                    tokens[i] = normalize(current);
                    current = normalize(current);

                    if (!isValid(current))
                        throw new FormulaFormatException("Variable is not valid according to isValid delegate. Check to make sure token at index " + i + " is a valid variable");
                }

                //Check if parenthesis, increment accordingly, and make sure leftParentheses < rightParentheses
                if (current.Equals("(")) openParentheses++;
                if (current.Equals(")")) closeParentheses++;
                if (closeParentheses > openParentheses)
                    throw new FormulaFormatException("Closed parenthesis found before open parenthesis at index " + i + ". Check formula input.");

                //Check if open parenthesis or operator, then makes sure next token is number, var, or opening parenthesis
                if (current.Equals("(") || isOperator(current))
                {
                    if (i < tokens.Length - 2)
                        throw new FormulaFormatException("Formula ends with open parenthesis or operator. Check formula input.");
                    if (!Double.TryParse(tokens[i + 1], out throwaway) && !Double.TryParse(tokens[i + 1], System.Globalization.NumberStyles.Float, null, out throwaway) && !Regex.IsMatch(tokens[i + 1], validVar) && !tokens[i + 1].Equals("("))
                    {
                        throw new FormulaFormatException("Invalid token follows open parenthesis or operator. Check to make sure token at index " + (i + 1) + " is a number, variable, or open parenthesis.");
                    }
                }

                //Check if number and varifies token afterwards is operator or closing parenthesis. And reassign token to toString'ed version of parsed double.
                if (Double.TryParse(current, out throwaway) || Double.TryParse(current, System.Globalization.NumberStyles.Float, null, out throwaway))
                {
                    tokens[i] = throwaway.ToString();
                    if (i >= tokens.Length - 2)
                        if (!tokens[i + 1].Equals(")") && !isOperator(tokens[i + 1]))
                            throw new FormulaFormatException("Invalid token follows number. Check to make sure token at index " + (i + 1) + " is closing parenthesis or operator.");
                }

                //Check if var or closing parenthesis and varifies token afterwards is operator or closing parenthesis
                if (Regex.IsMatch(current, validVar) || current.Equals(")"))
                {
                    if (i >= tokens.Length - 2)
                        if (!tokens[i + 1].Equals(")") && !isOperator(tokens[i + 1]))
                            throw new FormulaFormatException("Invalid token follows variable or closing parenthesis. Check to make sure token at index " + (i + 1) + " is closing parenthesis or operator.");
                }
            }

            //Check if final token was valid
            if (!Double.TryParse(tokens[tokens.Length - 1], out throwaway) && !Double.TryParse(tokens[tokens.Length - 1], System.Globalization.NumberStyles.Float, null, out throwaway) && !Regex.IsMatch(tokens[tokens.Length - 1], validVar) && !tokens[tokens.Length - 1].Equals(")"))
                throw new FormulaFormatException("The final token of the given string is invalid. Check formula input.");

            //Check if parentheses are equal
            if (openParentheses != closeParentheses)
                throw new FormulaFormatException("Total number of open parentheses does not equal total number of close parentheses after parsing tokens. Check formula input.");
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
            //Makes stacks for doubles and operators
            Stack<double> values = new Stack<double>();
            Stack<char> operators = new Stack<char>();
            object error = null;

            //Goes through tokens to evaluate expression
            foreach (string s in tokens)
            {
                error = null;
                //If s is a double
                if (Double.TryParse(s, out double tempDouble) || Double.TryParse(s, System.Globalization.NumberStyles.Float, null, out tempDouble))
                {
                    //Adds num2 to values for helper method
                    values.Push(tempDouble);
                    MiniEquate(values, operators, false, ref error);
                    if (!(error is null))
                        return error;
                }

                //If s is variable
                else if (Regex.IsMatch(s, validVar))
                {
                    double num2;
                    //Check if var exists
                    try
                    {
                        num2 = lookup(s);
                    }
                    catch
                    {
                        return new FormulaError("Variable " + s + " not found using lookup function.");
                    }

                    values.Push(num2);
                    MiniEquate(values, operators, false, ref error);
                    if (!(error is null))
                        return error;
                }

                //If s is + or -
                else if (char.TryParse(s, out char tempChar) && (tempChar == '+' || tempChar == '-'))
                {
                    char c2 = tempChar;
                    MiniEquate(values, operators, true, ref error);
                    if (!(error is null))
                        return error;
                    operators.Push(c2);
                }

                //If s is * or /
                else if (char.TryParse(s, out tempChar) && (tempChar == '*' || tempChar == '/'))
                    operators.Push(tempChar);

                //If s is (
                else if (char.TryParse(s, out tempChar) && tempChar == '(')
                    operators.Push(tempChar);

                //If s is )
                else if (char.TryParse(s, out tempChar) && tempChar == ')')
                {
                    //If + or - is at top of operator stack
                    if (MiniEquate(values, operators, true, ref error))
                    {
                        if (!(error is null))
                            return error;
                        //Make sure ( is next in stack
                        if (operators.TryPeek(out char openBrack) && openBrack == '(')
                            operators.Pop();
                        else
                            return new FormulaError("Expected '(' not found.");
                    }
                    else if (operators.TryPeek(out char openBrack) && openBrack == '(')
                    {
                        if (!(error is null))
                            return error;
                        operators.Pop();
                    }

                    //If * or / is on operator stack
                    MiniEquate(values, operators, false, ref error);
                    if (!(error is null))
                        return error;
                }
                //Otherwise, throw exception
                else
                    return new FormulaError("Invalid token found");
            }
            //If operator stack is empty
            if (operators.Count == 0 && values.Count == 1)
                return values.Pop();

            //If operator stack is not empty
            else if (operators.Count == 1 && (operators.Peek() == '+' || operators.Peek() == '-') && values.Count == 2)
            {
                error = null;
                double temp =  Solve(values.Pop(), values.Pop(), operators.Pop(), ref error);
                if (!(error is null))
                    return error;
                else
                    return temp;
            }
            else
                return new FormulaError("Invalid expression, unable to compelete operation.");
        }

        /// <summary>
        /// Gets numbers and operator to find result
        /// </summary>
        /// <param name="num2">Second number of equation</param>
        /// <param name="num1">First number of equation</param>
        /// <param name="oper">Operator to be applied to nums</param>
        /// <exception cref="System.ArgumentException">Thrown when trying to divide by zero</exception>
        /// <returns>Value found from the equation</returns>
        private static double Solve(double num2, double num1, char oper, ref object error)
        {
            switch (oper)
            {
                case '+':
                    return num1 + num2;

                case '-':
                    return num1 - num2;

                case '*':
                    return num1 * num2;

                case '/':
                    if (num2 == 0)
                    {
                        error = new FormulaError("Attempted to divide by zero.");
                        return 0;
                    }
                    else
                        return num1 / num2;
            }
            return 0;
        }

        /// <summary>
        /// Adds, subtracts, multiply, or divide when called
        /// </summary>
        /// <param name="values">Values stack to be used</param>
        /// <param name="operators">Operators stack to be used</param>
        /// <param name="addSub">Tells method if equating addition and subtraction or multiplication and division</param>
        /// <exception cref="System.ArgumentException">Thrown when value stack does not have enough values</exception>
        /// <returns>If operation was successful</returns>
        private static bool MiniEquate(Stack<double> values, Stack<char> operators, bool addSub, ref object error)
        {
            if (addSub)
            {
                //If + or - on operator stack
                if (operators.TryPeek(out char c1) && (c1 == '+' || c1 == '-'))
                {
                    //If 2+ values exist in values stack
                    if (values.Count > 1)
                    {
                        values.Push(Solve(values.Pop(), values.Pop(), operators.Pop(), ref error));
                        if (error is null)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        error = new FormulaError("Attempting to add or subtract with less than 2 values in the values stack.");
                        return false;
                    }
                }
            }
            else
            {
                //Make sure value is on stack
                if (!values.TryPop(out double num2))
                {
                    error = new FormulaError("Multiplication or division symbol found with insufficient values to equate.");
                    return false;
                }

                //If * or / is on operator stack
                if (operators.TryPeek(out char c) && (c == '*' || c == '/'))
                {
                    operators.Pop();
                    //Pop value and do operand
                    if (values.TryPeek(out double num1))
                    {
                        values.Push(Solve(num2, values.Pop(), c, ref error)); ;
                        if (error is null)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        error = new FormulaError("Multiplication or division symbol found with insufficient values to equate.");
                        return false;
                    }

                }
                //Otherwise, just add to values stack
                else
                    values.Push(num2);
            }
            return false;
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
            HashSet<string> result = new HashSet<string>();
            for (int i = 0; i < tokens.Length; i++)
                if (Regex.IsMatch(tokens[i], validVar))
                    result.Add(tokens[i]);
            return result;
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
            string result = "";
            for (int i = 0; i < tokens.Length; i++)
                result += tokens[i];
            return result;
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
            Formula compare = (Formula) obj;
            if (tokens.Length != compare.tokens.Length)
                return false;

            return this.ToString() == compare.ToString();

            ////Go through tokens to make sure all are equal
            //for(int i = 0; i < tokens.Length; i++)
            //{
            //    //If token is double
            //    if (Double.TryParse(tokens[i], out double tempFirst) || Double.TryParse(tokens[i], System.Globalization.NumberStyles.Float, null, out tempFirst))
            //    {
            //        if (Double.TryParse(compare.tokens[i], out double tempSecond) || Double.TryParse(compare.tokens[i], System.Globalization.NumberStyles.Float, null, out tempSecond))
            //            if (tempFirst.ToString() != tempSecond.ToString())
            //                return false;
            //            else
            //                continue;
            //        else
            //            return false;
            //    }
            //    //Otherwise
            //    else if ((tokens[i].Equals(compare.tokens[i])))
            //        continue;
            //    else
            //        return false;
            //}
            //return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
                return true;
            if ((f1 is null && !(f2 is null)) || (f2 is null && !(f1 is null)))
                return false;
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
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

        /// <summary>
        /// Checks if string is an operator (+, -, *, /)
        /// </summary>
        /// <param name="s">String to be checked</param>
        /// <returns>If string s is an operator</returns>
        private bool isOperator(string s)
        {
            return (s.Equals("+") || s.Equals("-") || s.Equals("*") || s.Equals("/"));
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
        public FormulaFormatException(String message)
            : base(message)
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
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static bool TryPeek<T>(this Stack<T> stack, out T result)
        {
            if (stack.Count > 0) {
                result = stack.Peek();
                return true;
            }
            result = default(T);
            return false;
        }

        public static bool TryPop<T>(this Stack<T> stack, out T result)
        {
            if (stack.Count > 0)
            {
                result = stack.Pop();
                return true;
            }
            result = default(T);
            return false;
        }
    }
}