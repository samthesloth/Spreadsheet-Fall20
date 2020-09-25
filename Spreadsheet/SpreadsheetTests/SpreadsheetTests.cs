using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        #region Simple tests

        [TestMethod]
        public void GetContentsOfNonExistentCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("", sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetContentsOfStringCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", "hi");
            Assert.AreEqual("hi", sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetContentsOfDoubleCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 5.0);
            Assert.AreEqual(5.0, sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetContentsOfFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("5+2"));
            Assert.AreEqual(new Formula("5 + 2"), sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void SimpleGetNamesOfNonEmptyCells()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", "hi");
            sheet.SetCellContents("A2", "hii");
            sheet.SetCellContents("A3", "hiii");
            sheet.SetCellContents("A4", "hiiii");
            sheet.SetCellContents("A5", "hiiiii");
            sheet.SetCellContents("A1", "");

            Assert.AreEqual(4, sheet.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void SetContentsThenSetEmptyThenCheckNonEmpty()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 5.0);
            sheet.SetCellContents("A1", "");
            Assert.AreEqual(0, sheet.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void SetContentsToFormulaOfExistentCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 5.0);
            sheet.SetCellContents("A1", new Formula("5 + 2"));
            Assert.AreEqual(new Formula("5+2"), sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void SimpleDependencyCheckWithSetCellContents()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 5.0);
            sheet.SetCellContents("B1", new Formula("A1 + 5"));
            sheet.SetCellContents("B2", new Formula("B1 + C2"));
            sheet.SetCellContents("A3", new Formula("B2 + 5"));
            sheet.SetCellContents("C2", new Formula("A1 + 1"));
            List<string> list = new List<string>(sheet.SetCellContents("A1", 2.0));
            List<string> check1 = new List<string>();
            List<string> check2 = new List<string>();
            check1.Add("A1"); check1.Add("C2"); check1.Add("B1"); check1.Add("B2"); check1.Add("A3");
            check2.Add("A1"); check2.Add("B1"); check2.Add("C2"); check2.Add("B2"); check2.Add("A3");
            Assert.IsTrue(list.SequenceEqual(check1) || list.SequenceEqual(check2));
        }

        #endregion Simple tests

        #region Exception tests

        /// <summary>
        /// GetCellContents exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("8_A");
        }

        /// <summary>
        /// Set cell to double exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDoubleNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, 5.0);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDoubleInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("%50", 5.0);
        }

        /// <summary>
        /// Set cell to text exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellTextNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, "hi");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellTextInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("$AH", "hi");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellTextNullText()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetCellContents("A1", s);
        }

        /// <summary>
        /// Set cell to Formula exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormulaNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, new Formula("5 + 2"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormulaInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("9", new Formula("32 - 1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellFormulaNullFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f = null;
            sheet.SetCellContents("A1", f);
        }

        /// <summary>
        /// Cyclic exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SimpleCycleWithSettingFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("B1 + 1"));
            sheet.SetCellContents("B1", new Formula("C1 + 1"));
            sheet.SetCellContents("C1", new Formula("A1 + 1"));
        }

        #endregion Exception tests

        #region Complicated Tests

        //Taken from Geoffrey on Piazza to more closely examine the problem
        [TestMethod]
        public void SetCellWithSpecificOrder()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = new Formula("A1*A1");
            Formula f2 = new Formula("B1+A1");
            Formula f3 = new Formula("B1-C1");

            sheet.SetCellContents("A1", 3);
            sheet.SetCellContents("B1", f1);
            sheet.SetCellContents("C1", f2);
            sheet.SetCellContents("D1", f3);

            List<string> test = new List<string>();
            test.Add("D1");
            Assert.IsTrue(test.SequenceEqual(sheet.SetCellContents("D1", f3)));

            test.Clear(); test.Add("A1"); test.Add("B1"); test.Add("C1"); test.Add("D1");
            Assert.IsTrue(test.SequenceEqual(sheet.SetCellContents("A1", 5.0)));
        }

        [TestMethod]
        public void StressTest()
        {
            //Creates spreadsheet and list of strings (from a0 to z25) to create the sheet
            AbstractSpreadsheet sheet = new Spreadsheet();
            List<string> cellsToMake = new List<string>();
            for (char c = 'a'; c <= 'z'; c++)
            {
                for (int i = 0; i < 26; i++)
                {
                    cellsToMake.Add("" + c + i);
                }
            }
            //Adds z26, as this will be our cell we change
            cellsToMake.Add("z26");

            //Adds cells (except z26) to spreadsheet, having each depend on cell after it (a0 depends on a1, etc)
            for (int i = 0; i < cellsToMake.Count - 1; i++)
                sheet.SetCellContents(cellsToMake[i], new Formula(cellsToMake[i + 1]));

            //Make new list that will be reverse of cellsToMake. This will be the list we expect to be returned when z26 is changed/set
            List<string> check = new List<string>();
            check.Add("z26");
            for (char c = 'z'; c >= 'a'; c--)
            {
                for (int i = 25; i >= 0; i--)
                {
                    check.Add("" + c + i);
                }
            }

            //Asserts if sequence of check equals list returned by SetCellContents of z26 to a double
            Assert.IsTrue(check.SequenceEqual(sheet.SetCellContents("z26", 5.0)));
        }

        #endregion Complicated Tests
    }
}