using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FormulaEvaluatorTest
{
    [TestClass]
    public class ExceptionTests
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
                case "p0":
                    return 0;
                default:
                    throw new NullReferenceException();
            }
        }



        [TestMethod]
        [ExpectedException (typeof(ArgumentException))]
        public void IntegerException1()
        {
            Evaluator.Evaluate(" * 5", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IntegerException2()
        {
            Evaluator.Evaluate("5 / 0", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VariableException1()
        {
            Evaluator.Evaluate("5 + b123", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VariableException2()
        {
            Evaluator.Evaluate("* A1", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VariableException3()
        {
            Evaluator.Evaluate("5 / p0", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddSubException1()
        {
            Evaluator.Evaluate("+ 5 + 3", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CloseParentheseException1()
        {
            Evaluator.Evaluate("(4+ )", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CloseParentheseException2()
        {
            Evaluator.Evaluate("5 + 3)", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CloseParentheseException3()
        {
            Evaluator.Evaluate("(5 / 0)", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FinalException1()
        {
            Evaluator.Evaluate("5 10", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FinalException2()
        {
            Evaluator.Evaluate("5 * 10 +", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TokenException1()
        {
            Evaluator.Evaluate("", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TokenException2()
        {
            Evaluator.Evaluate("/", Vars);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TokenException3()
        {
            Evaluator.Evaluate(null, Vars);
        }
    }
}
