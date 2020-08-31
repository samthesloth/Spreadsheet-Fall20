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

        //Test

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

            //Goes through tokens to evaluate expression
            foreach(string s in tokens)
            {

            }

            return 0;
        }
    }
}
