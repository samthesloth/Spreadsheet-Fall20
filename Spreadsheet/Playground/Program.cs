using System;
using System.Text.RegularExpressions;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(testmethod(2.0));
            Console.Read();
        }

        private static bool testmethod(object o)
        {
            return (o is double);
        }
    }
}
