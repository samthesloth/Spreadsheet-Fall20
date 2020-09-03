using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FormulaEvaluatorTest
{
    [TestClass]
    public class HeavyTests
    {

        private static int Vars(string s)
        {
            switch (s)
            {
                case "A1":
                    return 1;
                case "AGSE1248":
                    return 2;
                case "asdf1234":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
