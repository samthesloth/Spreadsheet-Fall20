using System;

namespace Playground
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            for (char c = 'a'; c <= 'z'; c++)
            {
                for (int i = 0; i < 26; i++)
                {
                    Console.WriteLine("" + c + i);
                }
            }
            Console.Read();
        }
    }
}