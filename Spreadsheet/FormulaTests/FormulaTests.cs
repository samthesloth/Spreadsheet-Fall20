using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void SimpleToString()
        {
            Formula f = new Formula("5 + 5");
            Assert.AreEqual("5+5", f.ToString());
        }

        [TestMethod]
        public void ToStringWithNormalizer()
        {
            Formula f = new Formula("A + b + _L1", s=>s.ToLower(), s=>true);
            Assert.AreEqual("a+b+_l1", f.ToString());
        }

        [TestMethod]
        public void SimpleEvaluate()
        {
            Formula f = new Formula("5 + 5");
            Assert.AreEqual(10.0, f.Evaluate(s => 1));
        }

        [TestMethod]
        public void SimpleScientificNotation()
        {
            Formula f = new Formula("1e2 + 2e2");
            Assert.AreEqual("100+200", f.ToString());
            Assert.AreEqual(300.0, f.Evaluate(s => 1));
        }

        [TestMethod]
        public void SimpleGetVariables()
        {
            Formula f = new Formula("A_ + 5 + _");
            List<string> list = (List<string>)f.GetVariables();
            Assert.IsTrue(list.Contains("A_") && list.Contains("_"));
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void SimpleEquals()
        {
            Formula f = new Formula("A + 5      - 2.00");
            Formula g = new Formula("B+5-2", s => "A", s=> true);
            Assert.AreEqual("A+5-2", f.ToString());
            Assert.AreEqual("A+5-2", g.ToString());
            Assert.IsTrue(f.Equals(g));
            Assert.IsTrue(f == g);
            Assert.IsFalse(f != g);
        }

        [TestMethod]
        public void SimpleGetHashCode()
        {
            Formula f = new Formula("A + 5      - 2.00");
            Formula g = new Formula("B+5-2", s => "A", s => true);
            Assert.AreEqual(f.GetHashCode(), g.GetHashCode());
        }
    }
}
