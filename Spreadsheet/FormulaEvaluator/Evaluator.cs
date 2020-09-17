using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /**
     *  This program is used to evaluate an integer expression, allowing for the use of variables found using a delegate
     *  Author: Sam Peters
     */

    public static class Evaluator
    {
        public static void Main(string[] args)
        {
        }

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
            //Makes sure string is not null or only white space
            if (string.IsNullOrWhiteSpace(exp))
                throw new ArgumentException("Input is Null or String with only whitespace / empty");

            //Makes array of tokens and stacks for ints and operators
            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> values = new Stack<int>();
            Stack<char> operators = new Stack<char>();

            //Goes through tokens to evaluate expression
            foreach (string token in tokens)
            {
                string s = token.Trim();

                if (string.IsNullOrWhiteSpace(s))
                    continue;

                //If s is an int
                if (int.TryParse(s, out int tempInt))
                {
                    //Adds num2 to values for helper method
                    values.Push(tempInt);

                    MiniEquate(values, operators, false);
                }

                //If s is variable
                else if (Regex.IsMatch(s, "^[a-zA-Z]+[0-9]+$"))
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

                    values.Push(num2);
                    MiniEquate(values, operators, false);
                }

                //If s is + or -
                else if (char.TryParse(s, out char tempChar) && (tempChar == '+' || tempChar == '-'))
                {
                    char c2 = tempChar;
                    MiniEquate(values, operators, true);
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
                    if (MiniEquate(values, operators, true))
                    {
                        //Make sure ( is next in stack
                        if (operators.TryPeek(out char openBrack) && openBrack == '(')
                            operators.Pop();
                        else
                            throw new ArgumentException("Expected '(' not found.");
                    }
                    else if (operators.TryPeek(out char openBrack) && openBrack == '(')
                        operators.Pop();

                    //If * or / is on operator stack
                    MiniEquate(values, operators, false);
                }
                //Otherwise, throw exception
                else
                    throw new ArgumentException("Invalid token found");
            }
            //If operator stack is empty
            if (operators.Count == 0 && values.Count == 1)
                return values.Pop();
            //If operator stack is not empty
            else if (operators.Count == 1 && (operators.Peek() == '+' || operators.Peek() == '-') && values.Count == 2)
            {
                return Solve(values.Pop(), values.Pop(), operators.Pop());
            }
            else
                throw new ArgumentException("Invalid expression, unable to compelete operation.");
        }

        /// <summary>
        /// Gets numbers and operator to find result
        /// </summary>
        /// <param name="num2">Second number of equation</param>
        /// <param name="num1">First number of equation</param>
        /// <param name="oper">Operator to be applied to nums</param>
        /// <exception cref="System.ArgumentException">Thrown when trying to divide by zero</exception>
        /// <returns>Value found from the equation</returns>
        private static int Solve(int num2, int num1, char oper)
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
                        throw new ArgumentException("Division by zero.");
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
        private static bool MiniEquate(Stack<int> values, Stack<char> operators, bool addSub)
        {
            if (addSub)
            {
                //If + or - on operator stack
                if (operators.TryPeek(out char c1) && (c1 == '+' || c1 == '-'))
                {
                    //If 2+ values exist in values stack
                    if (values.Count > 1)
                    {
                        values.Push(Solve(values.Pop(), values.Pop(), operators.Pop()));
                        return true;
                    }
                    else
                        throw new ArgumentException("Attempting to add or subtract with less than 2 values in the values stack.");
                }
            }
            else
            {
                //Make sure value is on stack
                if (!values.TryPop(out int num2))
                    throw new ArgumentException("Multiplying or dividing with empty values stack");

                //If * or / is on operator stack
                if (operators.TryPeek(out char c) && (c == '*' || c == '/'))
                {
                    operators.Pop();
                    //Pop value and do operand
                    if (values.TryPeek(out int num1))
                    {
                        values.Push(Solve(num2, values.Pop(), c)); ;
                        return true;
                    }
                    else
                        throw new ArgumentException("Adding or dividing with empty value stack.");
                }
                //Otherwise, just add to values stack
                else
                    values.Push(num2);
            }
            return false;
        }
    }
}