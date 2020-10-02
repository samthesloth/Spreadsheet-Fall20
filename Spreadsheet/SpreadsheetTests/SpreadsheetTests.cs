using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        #region Simple tests

        /// <summary>
        /// GetCellContents Test
        /// </summary>
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
            sheet.SetContentsOfCell("A1", "hi");
            Assert.AreEqual("hi", sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetContentsOfDoubleCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5.0");
            Assert.AreEqual(5.0, sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetContentsOfFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=1+1");
            sheet.SetContentsOfCell("A1", "=5+2");
            Assert.AreEqual(new Formula("5 + 2"), sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// SetContents tests
        /// </summary>
        [TestMethod]
        public void SetContentsThenSetEmptyThenCheckNonEmpty()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5.0");
            sheet.SetContentsOfCell("A1", "4.0");
            sheet.SetContentsOfCell("A1", "");
            Assert.AreEqual(0, sheet.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void SetContentsToFormulaOfExistentCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5.0");
            sheet.SetContentsOfCell("A1", "=5 + 2");
            Assert.AreEqual(new Formula("5+2"), sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// GetValue tests
        /// </summary>
        [TestMethod]
        public void EvaluateFormulaWithNonexistentVar()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1");
            Assert.IsTrue(sheet.GetCellValue("A1") is FormulaError);
            sheet.SetContentsOfCell("B1", "5.0");
            Assert.AreEqual(sheet.GetCellValue("B1"), sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetValueNonexistentCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("", sheet.GetCellValue("a1"));
        }

        [TestMethod]
        public void GetValueDouble()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "5.0");
            Assert.AreEqual(5.0, sheet.GetCellValue("a1"));
        }

        [TestMethod]
        public void GetValueString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hi");
            Assert.AreEqual("hi", sheet.GetCellValue("a1"));
        }

        [TestMethod]
        public void GetValueFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=(5.0 + 2 - 1) * 5");
            sheet.SetContentsOfCell("a2", "=a1");
            Assert.AreEqual(30.0, sheet.GetCellValue("a2"));
        }

        /// <summary>
        /// Save, load, versioning tests
        /// </summary>

        [TestMethod]
        public void ChangedWhenMadeSavedAndChanged()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.IsFalse(sheet.Changed);
            sheet.SetContentsOfCell("A1", "5.0");
            Assert.IsTrue(sheet.Changed);
            sheet.Save("changed.txt");
            Assert.IsFalse(sheet.Changed);
            sheet.SetContentsOfCell("A1", "=5 + 2");
            Assert.AreEqual(new Formula("5+2"), sheet.GetCellContents("A1"));
            sheet.SetContentsOfCell("B2", "hi");
            sheet.SetContentsOfCell("C1", "5.0");
            sheet.Save("changed.txt");
        }

        [TestMethod]
        public void FourArgumentConstructorWithNullDelegatesAndTestLoad()
        {
            AbstractSpreadsheet sheet1 = new Spreadsheet();
            sheet1.SetContentsOfCell("A1", "=5 + 2");
            sheet1.SetContentsOfCell("B2", "hi");
            sheet1.SetContentsOfCell("C1", "5.0");
            sheet1.Save("other.txt");
            AbstractSpreadsheet sheet2 = new Spreadsheet("other.txt", null, null, "default");
            Assert.AreEqual(new Formula("5+2"), sheet2.GetCellContents("A1"));
            Assert.AreEqual("hi", sheet2.GetCellContents("B2"));
            Assert.AreEqual(5.0, sheet2.GetCellContents("C1"));
        }

        [TestMethod]
        public void CheckingVersion()
        {
            AbstractSpreadsheet sheet1 = new Spreadsheet(null, null, "1.0");
            sheet1.SetContentsOfCell("A1", "5.0");
            sheet1.Save("woah.txt");
            Assert.AreEqual("1.0", sheet1.GetSavedVersion("woah.txt"));
        }

        /// <summary>
        /// Other simple tests
        /// </summary>
        [TestMethod]
        public void SimpleGetNamesOfNonEmptyCells()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "hi");
            sheet.SetContentsOfCell("A2", "hii");
            sheet.SetContentsOfCell("A3", "hiii");
            sheet.SetContentsOfCell("A4", "hiiii");
            sheet.SetContentsOfCell("A5", "hiiiii");
            sheet.SetContentsOfCell("A1", "");

            Assert.AreEqual(4, sheet.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void SimpleDependencyCheckWithSetCellContents()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5.0");
            sheet.SetContentsOfCell("B1", "=A1 + 5");
            sheet.SetContentsOfCell("B2", "=B1 + C2");
            sheet.SetContentsOfCell("A3", "=B2 + 5");
            sheet.SetContentsOfCell("C2", "=A1 + 1");
            List<string> list = new List<string>(sheet.SetContentsOfCell("A1", "2.0"));
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
        public void GetContentsNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetContentsInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("8_A");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetContentsInvalidNormalizedIsValid()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => s.Any(char.IsUpper), s => s.ToLower(), "1.0");
            sheet.GetCellContents("a1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetContentsInvalidNormalized()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => "%$", "1.0");
            sheet.GetCellContents("a1");
        }

        /// <summary>
        /// GetValue exception tests
        /// </summary>

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellValue(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellValue("%$");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueInvalidNormalizedIsValid()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => s.Any(char.IsUpper), s => s.ToLower(), "1.0");
            sheet.GetCellValue("a1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsInvalidNormalized()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => "%$", "1.0");
            sheet.GetCellValue("a1");
        }

        /// <summary>
        /// Set cell to double exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDoubleNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "5.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellDoubleInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("%50", "5.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellInvalidNormalizedIsValid()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => s.Any(char.IsUpper), s => s.ToLower(), "1.0");
            sheet.SetContentsOfCell("a1", "5.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellInvalidNormalized()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => "%$", "1.0");
            sheet.SetContentsOfCell("a1", "5.0");
        }

        /// <summary>
        /// Set cell to text exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellTextNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "hi");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellTextInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("$AH", "hi");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellTextNullText()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetContentsOfCell("A1", s);
        }

        /// <summary>
        /// Set cell to Formula exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormulaNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "=5 + 2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellFormulaInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("9", "=32 - 1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetCellFormulaInvalidFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=(((5 + 1))");
        }

        /// <summary>
        /// Cyclic exceptions
        /// </summary>
        [TestMethod()]
        public void SimpleCycleWithSettingFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1 + 1");
            sheet.SetContentsOfCell("B1", "=C1 + 1");
            Assert.ThrowsException<CircularException>(() => sheet.SetContentsOfCell("C1", "=A1 + 1"));
            Assert.AreEqual("", sheet.GetCellContents("C1"));
        }

        /// <summary>
        /// Save/Load exceptions
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadingWithInvalidVersion()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("changed.txt", null, null, "asdfalsg");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadingWithInvalidCellName()
        {
            makeIncorrectXmlCellName("invalidcell.txt");
            AbstractSpreadsheet sheet = new Spreadsheet("invalidcell.txt", null, null, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void LoadingWithInvalidFile()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("/nonexistent/HITHISISNOTAREALFILE.txt", null, null, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SavingInvalidFile()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hi");
            sheet.Save("/nonexistent/whoop.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GettingVersionOfXmlWithNoVersion()
        {
            makeIncorrectXmlNoVersion("woahoof.txt");
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetSavedVersion("woahoof.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GettingVersionOfXmlWithInvalidElement()
        {
            makeIncorrectXmlElement("invalidelement.txt");
            AbstractSpreadsheet sheet = new Spreadsheet("invalidelement.txt", null, null, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GettingVersionOfInvalidFile()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetSavedVersion("/nonexistent/whoop.txt");
        }

        #region XML Helper Methods

        /// <summary>
        /// Creates an xml file with the given file name that contains spreadsheet element with no version attribute
        /// </summary>
        /// <param name="filename"></param>
        private void makeIncorrectXmlNoVersion(string filename)
        {
            //Sets up xmlwriter
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            XmlWriter writer = null;

            //Write file
            try
            {
                using (writer = XmlWriter.Create(filename, settings))
                {
                    //Top of document
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");

                    //End
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Problem saving xml file. Check if filename is valid.");
            }
        }

        /// <summary>
        /// Makes an xml file with the given file name with an invalid cell name
        /// </summary>
        /// <param name="filename"></param>
        private void makeIncorrectXmlCellName(string filename)
        {
            //Sets up xmlwriter
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            XmlWriter writer = null;

            //Write file
            try
            {
                using (writer = XmlWriter.Create(filename, settings))
                {
                    //Makes doc
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "default");
                    writer.WriteStartElement("cell");
                    writer.WriteStartElement("name");
                    writer.WriteValue("%sdfg");
                    writer.WriteEndElement();
                    writer.WriteStartElement("contents");
                    writer.WriteValue("5.0");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Problem saving xml file. Check if filename is valid.");
            }
        }

        /// <summary>
        /// Makes an xml file with the given file name with an invalid xml element
        /// </summary>
        /// <param name="filename"></param>
        private void makeIncorrectXmlElement(string filename)
        {
            //Sets up xmlwriter
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            XmlWriter writer = null;

            //Write file
            try
            {
                using (writer = XmlWriter.Create(filename, settings))
                {
                    //Makes doc
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Spreadsheet");
                    writer.WriteAttributeString("version", "default");
                    writer.WriteStartElement("HiThere");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Problem saving xml file. Check if filename is valid.");
            }
        }

        #endregion XML Helper Methods

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

            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=A1*A1");
            sheet.SetContentsOfCell("C1", "=B1+A1");
            sheet.SetContentsOfCell("D1", "=B1-C1");

            List<string> test = new List<string>();
            test.Add("D1");
            Assert.IsTrue(test.SequenceEqual(sheet.SetContentsOfCell("D1", "=B1-C1")));

            test.Clear(); test.Add("A1"); test.Add("B1"); test.Add("C1"); test.Add("D1");
            Assert.IsTrue(test.SequenceEqual(sheet.SetContentsOfCell("A1", "5.0")));
        }

        //[TestMethod]
        //public void StressTest()
        //{
        //    //Creates spreadsheet and list of strings (from a0 to z25) to create the sheet
        //    AbstractSpreadsheet sheet = new Spreadsheet();
        //    List<string> cellsToMake = new List<string>();
        //    for (char c = 'a'; c <= 'z'; c++)
        //    {
        //        for (int i = 0; i < 26; i++)
        //        {
        //            cellsToMake.Add("" + c + i);
        //        }
        //    }
        //    //Adds z26, as this will be our cell we change
        //    cellsToMake.Add("z26");

        //    //Adds cells (except z26) to spreadsheet, having each depend on cell after it (a0 depends on a1, etc)
        //    for (int i = 0; i < cellsToMake.Count - 1; i++)
        //        sheet.SetContentsOfCell(cellsToMake[i], "=" + cellsToMake[i + 1]);

        //    //Make new list that will be reverse of cellsToMake. This will be the list we expect to be returned when z26 is changed/set
        //    List<string> check = new List<string>();
        //    check.Add("z26");
        //    for (char c = 'z'; c >= 'a'; c--)
        //    {
        //        for (int i = 25; i >= 0; i--)
        //        {
        //            check.Add("" + c + i);
        //        }
        //    }

        //    //Asserts if sequence of check equals list returned by SetCellContents of z26 to a double
        //    Assert.IsTrue(check.SequenceEqual(sheet.SetContentsOfCell("z26", "5.0")));

        //    //Checks if all cells got 5.0 as value
        //    foreach(string s in sheet.GetNamesOfAllNonemptyCells())
        //    {
        //        Assert.AreEqual(5.0, sheet.GetCellValue(s));
        //    }
        //}

        #endregion Complicated Tests
    }
}