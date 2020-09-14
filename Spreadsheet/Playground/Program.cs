using System;
using System.Text.RegularExpressions;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            string validVar = "^[a-zA-Z_]+[0-9a-zA-Z_]*$";

            Console.WriteLine(Regex.IsMatch("a_5_a3&", validVar));
            Console.Read();
        }
    }
}
