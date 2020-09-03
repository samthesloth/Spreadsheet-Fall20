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

        [TestMethod]
        public void Parentheses1()
        {
            Assert.AreEqual(Evaluator.Evaluate("(4 * 3 + 5) / (1 + 2 - 1)", Vars), 8);
        }

        [TestMethod]
        public void Parentheses2()
        {
            Assert.AreEqual(Evaluator.Evaluate("1(+1)", Vars), 2);
        }

        [TestMethod]
        public void Parentheses3()
        {
            Assert.AreEqual(Evaluator.Evaluate("(1) (1) +", Vars), 2);
        }


        [TestMethod]
        public void Complicated1()
        {
            Assert.AreEqual(Evaluator.Evaluate("((200 / 5) * (81 / 9)) / 4) * 10", Vars), 900);
        }

        [TestMethod]
        public void Complicated2()
        {
            Assert.AreEqual(Evaluator.Evaluate("(50 + 3-(123 / 5) * 3 +(5 * 1) - 15) * 7", Vars), -203);
        }

    }
}
