using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
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
        public void SimplyDependencyCheckWithSetCellContents()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 5.0);
            sheet.SetCellContents("B1", new Formula("A1 + 5"));
            sheet.SetCellContents("B2", new Formula("B1 + C2"));
            sheet.SetCellContents("A3", new Formula("B2 + 5"));
            sheet.SetCellContents("C2", new Formula("A1 + 1"));
            List<string> list = sheet.SetCellContents("A1", 2.0).ToList();
            foreach (string s in list)
            {
                System.Console.WriteLine(s);
            }
            System.Console.Read();
        }
    }
}
