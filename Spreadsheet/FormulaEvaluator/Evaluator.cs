using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Takes in a string and evaluates the integer arithmetic expressions using standard infix notation
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Delegate that looks up the int value of a variable, v
        /// </summary>
        /// <param name="v">Variable that needs to be looked up</param>
        /// <returns>Int value of the variable. Throws ArgumentException if the variable has no value.</returns>
        public delegate int Lookup(string v);

        /// <summary>
        /// Takes in a string and delegate to find the value of the expression
        /// </summary>
        /// <param name="exp">String to be evaluated</param>
        /// <param name="variableEvaluator">Delegate with method to find values of variables</param>
        /// <exception cref="System.ArgumentException">Thrown when invalid expression is given</exception>
        /// <returns>Int solution of the expression</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            //Makes array of tokens and stacks for ints and operators
            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> values = new Stack<int>();
            Stack<char> operators = new Stack<char>();
            char tempChar;
            int tempInt;
            int i;

            //Goes through tokens to evaluate expression

            for (i = 0; i < tokens.Length; i++)
            {
                string s = tokens[i];
                s = s.Trim();
                if (string.IsNullOrWhiteSpace(s))
                    continue;

                //If s is an int
                if (int.TryParse(s, out tempInt))
                {
                    int num2 = tempInt;
                    //If * or / is on operator stack
                    if (operators.TryPeek(out char c) && (c == '*' || c == '/'))
                    {
                        operators.Pop();
                        //Pop value and do operand
                        if (values.TryPop(out int num1))
                        {
                            if (c == '/' && num1 == 0)
                                throw new ArgumentException("Dividing by 0.");
                            else
                            {
                                if (c == '/')
                                    values.Push(num1 / num2);
                                else
                                    values.Push(num1 * num2);
                            }
                        }
                        else
                            throw new ArgumentException("Adding or dividing with empty value stack.");
                    }
                    //Otherwise, just add to values stack
                    else
                        values.Push(num2);
                }

                //If s is variable
                else if (Regex.IsMatch(s, @"^[a-zA-Z]+$"))
                {
                    s = GetVar(s, i, tokens);
                    i += s.Length - 1;
                    int num2;
                    //Check if var exists
                    try
                    {
                        num2 = variableEvaluator(s);
                    }
                    catch
                    {
                        throw new ArgumentException("Value for variable " + s + " cannot be found.");
                    }

                    //If * or / is on operator stack
                    if (operators.TryPeek(out char c) && (c == '*' || c == '/'))
                    {
                        operators.Pop();
                        //Pop value and do operand
                        if (values.TryPop(out int num1))
                        {
                            if (c == '/' && num2 == 0)
                                throw new ArgumentException("Dividing by 0.");
                            else
                            {
                                if (c == '/')
                                    values.Push(num1 / num2);
                                else
                                    values.Push(num1 * num2);
                            }
                        }
                        else
                            throw new ArgumentException("Adding or dividing with empty value stack.");
                    }
                    //Otherwise, just add to values stack
                    else
                        values.Push(num2);
                }

                //If s is + or -
                else if (char.TryParse(s, out tempChar) && (tempChar == '+' || tempChar == '-'))
                {
                    char c2 = tempChar;

                    //If + or - on operator stack
                    if (operators.TryPeek(out char c1) && (c1 == '+' || c1 == '-'))
                    {
                        //If 2+ values exist in values stack
                        if (values.Count > 1)
                        {
                            operators.Pop();
                            int num2 = values.Pop();
                            int num1 = values.Pop();
                            if (c1 == '+')
                                values.Push(num1 + num2);
                            else
                                values.Push(num1 - num2);
                        }
                        else
                            throw new ArgumentException("Attempting to add or subtract with less than 2 values in the values stack.");
                    }
                    operators.Push(c2);
                }

                //If s is * or /
                else if (char.TryParse(s, out tempChar) && (tempChar == '*' || tempChar == '/'))
                    operators.Push(tempChar);

                //If s is (
                else if (char.TryParse(s, out tempChar) && tempChar == ')')
                    operators.Push(tempChar);

                //If s is )
                else if (char.TryParse(s, out tempChar) && tempChar == '(')
                {
                    //If + or - is at top of operator stack
                    if (operators.TryPeek(out char c1) && (c1 == '+' || c1 == '-'))
                    {
                        //If 2+ values exist in values stack
                        if (values.Count > 1)
                        {
                            operators.Pop();
                            int num2 = values.Pop();
                            int num1 = values.Pop();
                            if (c1 == '+')
                                values.Push(num1 + num2);
                            else
                                values.Push(num1 - num2);

                            //Make sure ( is next in stack
                            if (operators.TryPeek(out char openBrack) && openBrack == '(')
                                operators.Pop();
                            else
                                throw new ArgumentException("Expected '(' not found.");
                        }
                        else
                            throw new ArgumentException("Attempting to add or subtract with less than 2 values in the values stack.");
                    }
                    //If * or / is on operator stack
                    if (operators.TryPeek(out char c) && (c == '*' || c == '/'))
                    {
                        operators.Pop();
                        //If 2+ values exist in values stack
                        if (values.Count > 1)
                        {
                            operators.Pop();
                            int num2 = values.Pop();
                            int num1 = values.Pop();
                            if (c == '/' && num2 == 0)
                                throw new ArgumentException("Dividing by 0.");
                            if (c1 == '*')
                                values.Push(num1 * num2);
                            else
                                values.Push(num1 / num2);
                        }
                        else
                            throw new ArgumentException("Attempting to multiply or divide with less than 2 values in the values stack.");
                    }
                }
            }
            //If operator stack is empty
            if (operators.Count == 0 && values.Count == 1)
                return values.Pop();
            //If operator stack is not empty
            else if (operators.Count == 1 && (operators.Peek() == '+' || operators.Peek() == '-') && values.Count == 2)
            {
                tempChar = operators.Pop();
                if (tempChar == '+')
                    return values.Pop() + values.Pop();
                else
                    return -values.Pop() + values.Pop();
            }
            else
                throw new ArgumentException("Invalid expression, unable to compelete operation");
        }

        /// <summary>
        /// Finds whole var in the tokens
        /// </summary>
        /// <param name="s">Starting string of variable</param>
        /// <param name="i">Index of starting string in tokens</param>
        /// <param name="tokens">Tokens from evaluator method</param>
        /// <exception cref="System.ArgumentException">Thrown when invalid variable is given</exception>
        /// <returns></returns>
        private static string GetVar(string s, int i, string[] tokens)
        {
            if (Regex.IsMatch(tokens[i + 1], @"^[a-zA-Z]+$") || int.TryParse(tokens[i + 1], out int temp))
            {
                while (Regex.IsMatch(tokens[i + 1], @"^[a-zA-Z]+$"))
                {
                    s += tokens[i + 1];
                    i++;
                }
                if (int.TryParse(tokens[i + 1], out temp))
                {
                    while (int.TryParse(tokens[i + 1], out temp))
                    {
                        s += tokens[i + 1];
                        i++;
                    }
                }
                else
                    throw new ArgumentException("Invalid variable starting with " + s);
            }
            else
                throw new ArgumentException("Invalid variable starting with " + s);
            return s;
        }
    }
}