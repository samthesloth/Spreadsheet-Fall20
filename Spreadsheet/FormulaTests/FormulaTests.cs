using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Formula f = new Formula("A + b + _L1", s => s.ToLower(), s => true);
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
            Formula g = new Formula("B+5-2", s => "A", s => true);
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

        [TestMethod]
        public void NullNormalizerAndValidator()
        {
            Formula f = new Formula("A + 5      - 2.00", null, null);
            Formula g = new Formula("A+5-2");
            Assert.IsTrue(f == g);
        }

        [TestMethod]
        public void EqualityWithBothNull()
        {
            Formula f = null;
            Formula g = null;
            Assert.IsTrue(f == g);
        }

        [TestMethod]
        public void EqualityWithFirstNull()
        {
            Formula f = null;
            Formula g = new Formula("5 + 2");
            Assert.IsFalse(f == g);
        }

        [TestMethod]
        public void EqualityWithSecondNull()
        {
            Formula f = null;
            Formula g = new Formula("5 + 2");
            Assert.IsFalse(g == f);
        }

        [TestMethod]
        public void SimpleInequalityWithDifferentLengths()
        {
            Formula f = new Formula("5 + 2 + 1");
            Formula g = new Formula("5 + 2");
            Assert.IsFalse(f.Equals(g));
        }

        /// <summary>
        /// Formula format exception tests
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleInvalidTokenWithNum()
        {
            Formula f = new Formula("5!");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleInvalidTokenAsSecondToken()
        {
            Formula f = new Formula("!");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleInvalidTokenWithSymbol()
        {
            Formula f = new Formula("A% + 5");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NoTokens()
        {
            Formula f = new Formula("    ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void MoreLeftParenthesesThanRight()
        {
            Formula f = new Formula("(5 + 3) + 5) + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ClosingParenethesisAsFirstToken()
        {
            Formula f = new Formula(")5 + 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorAsFirstToken()
        {
            Formula f = new Formula("* (5 + 3)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorAsLastToken()
        {
            Formula f = new Formula("abc + 4 *");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OpeningParenthesisAsLastToken()
        {
            Formula f = new Formula("1.0 * 2.0 (");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ClosingParenthesisAfterOpening()
        {
            Formula f = new Formula("()");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorAfterOpeningParenthesis()
        {
            Formula f = new Formula("5 ( * 5)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ClosingParenthesisAfterOperator()
        {
            Formula f = new Formula("(5 * ) 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorAfterOperator()
        {
            Formula f = new Formula("5 -- 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NumberAfterNumber()
        {
            Formula f = new Formula("5 5 + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OpenParenthesisAfterNumber()
        {
            Formula f = new Formula("5 ( 4 * 2)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NumberAfterVariable()
        {
            Formula f = new Formula("a5 2 - 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NumberAfterClosingParenthesis()
        {
            Formula f = new Formula("(1 + 2 + 3) 4");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NotValidVar()
        {
            Formula f = new Formula("A5 + 2", s=>s.ToLower(), s=>s.Any(char.IsUpper));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParenthesisNotEqualAtEnd()
        {
            Formula f = new Formula("(A5+A+(5)");
        }

        /// <summary>
        /// Formula error tests
        /// </summary>
        [TestMethod]
        public void DivideByZero()
        {
            Assert.IsInstanceOfType(new Formula("5 / 0").Evaluate(s => 1), typeof(FormulaError));
        }

        private double lookup(string s)
        {
            if (s.Equals("A"))
                return 1.0;
            else if (s.Equals("X"))
                return 0.0;
            else if (s.Equals("a"))
                return 10.0;
            else
                throw new ArgumentException();
        }

        [TestMethod]
        public void DivideByZeroWithVar()
        {
            Assert.IsInstanceOfType(new Formula("5 / X").Evaluate(lookup), typeof(FormulaError));
        }

        [TestMethod()]
        public void ClosingParenthesisDivideByZero()
        {
            Formula f = new Formula("(2+6)/0");
            FormulaError e = (FormulaError)f.Evaluate(s => 0.0);
            Assert.AreEqual("Attempted to divide by zero.", e.Reason);
            Assert.IsInstanceOfType(new Formula("(2+6)/0").Evaluate(s => 0.0), typeof(FormulaError));
        }

        [TestMethod]
        public void UnknownVar()
        {
            Assert.IsInstanceOfType(new Formula("5 * b").Evaluate(lookup), typeof(FormulaError));
        }

        /// <summary>
        ///Altered tests from ps1, given by professor and some made by me
        ///</summary>
        [TestMethod(), Timeout(5000)]
        [TestCategory("1")]
        public void TestSingleNumber()
        {
            Assert.AreEqual(5.0, new Formula("5").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("2")]
        public void TestSingleVariable()
        {
            Assert.AreEqual(13.0, new Formula("X5").Evaluate(s => 13.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("3")]
        public void TestAddition()
        {
            Assert.AreEqual(8.0, new Formula("5+3").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("4")]
        public void TestSubtraction()
        {
            Assert.AreEqual(8.0, new Formula("18-10").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("5")]
        public void TestMultiplication()
        {
            Assert.AreEqual(8.0, new Formula("2*4").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("6")]
        public void TestDivision()
        {
            Assert.AreEqual(8.0, new Formula("16/2").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("7")]
        public void TestArithmeticWithVariable()
        {
            Assert.AreEqual(6.0, new Formula("2+X1").Evaluate(s => 4.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("9")]
        public void TestLeftToRight()
        {
            Assert.AreEqual(15.0, new Formula("2*6+3").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("10")]
        public void TestOrderOperations()
        {
            Assert.AreEqual(20.0, new Formula("2+6*3").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("11")]
        public void TestParenthesesTimes()
        {
            Assert.AreEqual(24.0, new Formula("(2+6)*3").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("12")]
        public void TestTimesParentheses()
        {
            Assert.AreEqual(16.0, new Formula("2*(3+5)").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("13")]
        public void TestPlusParentheses()
        {
            Assert.AreEqual(10.0, new Formula("2+(3+5)").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("14")]
        public void TestPlusComplex()
        {
            Assert.AreEqual(50.0, new Formula("2+(3+5*9)").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("15")]
        public void TestOperatorAfterParens()
        {
            Assert.AreEqual(1.0, new Formula("(1*2)-2/2").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("16")]
        public void TestComplexTimesParentheses()
        {
            Assert.AreEqual(26.0, new Formula("2+3*(3+5)").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("17")]
        public void TestComplexAndParentheses()
        {
            Assert.AreEqual(194.0, new Formula("2+3*5+(3+4*8)*5+2").Evaluate(s => 0.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("21")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("23")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestPlusInvalidVariable()
        {
            Formula f = new Formula("5+0x");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("24")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestParensNoOperator()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("25")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("27")]
        public void TestComplexNestedParensRight()
        {
            Assert.AreEqual(6.0, new Formula("x1+(x2+(x3+(x4+(x5+x6))))").Evaluate(s => 1.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("28")]
        public void TestComplexNestedParensLeft()
        {
            Assert.AreEqual(12.0, new Formula("((((x1+x2)+x3)+x4)+x5)+x6").Evaluate(s => 2.0));
        }

        [TestMethod(), Timeout(5000)]
        [TestCategory("29")]
        public void TestRepeatedVar()
        {
            Assert.AreEqual(0.0, new Formula("a4-a4*a4/a4").Evaluate(s => 3));
        }
    }
}