using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
        /// <returns>Int solution of the expression. Throws ArgumentException if unable to find result</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            //Makes array of tokens and stacks for ints and operators
            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> values = new Stack<int>();
            Stack<char> operators = new Stack<char>();
            char tempChar;
            int tempInt;

            //Goes through tokens to evaluate expression
            foreach(string s in tokens)
            {
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
                    //TODO
                }
            }

            return 0;
        }
    }
}
