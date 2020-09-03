using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaEvaluatorTest
{
    [TestClass]
    public class BasicTests
    {

        private static int Vars(string s)
        {
            switch(s)
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
        public void Addition()
        {
            Assert.AreEqual(Evaluator.Evaluate("5 + 3", Vars), 8);
        }

        [TestMethod]
        public void Subtraction()
        {
            Assert.AreEqual(Evaluator.Evaluate("  1     -     3 ", Vars), -2);
        }

        [TestMethod]
        public void Multiplication()
        {
            Assert.AreEqual(Evaluator.Evaluate("5*3", Vars), 15);
        }

        [TestMethod]
        public void Division()
        {
            Assert.AreEqual(Evaluator.Evaluate("5 / 2", Vars), 2);
        }

        [TestMethod]
        public void Variables()
        {
            Assert.AreEqual(Evaluator.Evaluate("5+A1-asdf1234+AGSE1248", Vars), 6);
        }

        [TestMethod]
        public void OrderOfOperations()
        {
            Assert.AreEqual(Evaluator.Evaluate("5 + 2 * 3 + 5", Vars), 16);
        }

        [TestMethod]
        public void Parentheses()
        {
            Assert.AreEqual(Evaluator.Evaluate("5 * (3 + 4) - 2 + (6*(2+3))", Vars), 63);
        }
    }
}
