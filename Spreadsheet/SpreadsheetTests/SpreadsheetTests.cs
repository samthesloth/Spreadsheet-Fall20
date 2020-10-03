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
        public void SimpleDependencyCheckWithSetContentsOfCell()
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
        public void SavingNullFileName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hi");
            sheet.Save(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SavingEmptyFileName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hi");
            sheet.Save("");
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

        //    //Asserts if sequence of check equals list returned by SetContentsOfCell of z26 to a double
        //    Assert.IsTrue(check.SequenceEqual(sheet.SetContentsOfCell("z26", "5.0")));

        //    //Checks if all cells got 5.0 as value
        //    foreach(string s in sheet.GetNamesOfAllNonemptyCells())
        //    {
        //        Assert.AreEqual(5.0, sheet.GetCellValue(s));
        //    }
        //}

        /// <summary>
        /// Stress tests taken from ps4 grading tests
        /// </summary>

        [TestMethod(), ]
        [TestCategory("31")]
        public void TestStress1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=B1+B2"));
            s.SetContentsOfCell("B1", ("=C1-C2"));
            s.SetContentsOfCell("B2", ("=C3*C4"));
            s.SetContentsOfCell("C1", ("=D1*D2"));
            s.SetContentsOfCell("C2", ("=D3*D4"));
            s.SetContentsOfCell("C3", ("=D5*D6"));
            s.SetContentsOfCell("C4", ("=D7*D8"));
            s.SetContentsOfCell("D1", ("=E1"));
            s.SetContentsOfCell("D2", ("=E1"));
            s.SetContentsOfCell("D3", ("=E1"));
            s.SetContentsOfCell("D4", ("=E1"));
            s.SetContentsOfCell("D5", ("=E1"));
            s.SetContentsOfCell("D6", ("=E1"));
            s.SetContentsOfCell("D7", ("=E1"));
            s.SetContentsOfCell("D8", ("=E1"));
            IList<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }

        // Repeated for extra weight
        [TestMethod(), ]
        [TestCategory("32")]
        public void TestStress1a()
        {
            TestStress1();
        }
        [TestMethod(), ]
        [TestCategory("33")]
        public void TestStress1b()
        {
            TestStress1();
        }
        [TestMethod(), ]
        [TestCategory("34")]
        public void TestStress1c()
        {
            TestStress1();
        }

        [TestMethod(), ]
        [TestCategory("35")]
        public void TestStress2()
        {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, ("=A" + (i + 1).ToString()))));
            }
        }
        [TestMethod(), ]
        [TestCategory("36")]
        public void TestStress2a()
        {
            TestStress2();
        }
        [TestMethod(), ]
        [TestCategory("37")]
        public void TestStress2b()
        {
            TestStress2();
        }
        [TestMethod(), ]
        [TestCategory("38")]
        public void TestStress2c()
        {
            TestStress2();
        }

        [TestMethod(), ]
        [TestCategory("39")]
        public void TestStress3()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, ("=A" + (i + 1).ToString()));
            }
            try
            {
                s.SetContentsOfCell("A150", ("=A50"));
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }

        [TestMethod(), ]
        [TestCategory("40")]
        public void TestStress3a()
        {
            TestStress3();
        }
        [TestMethod(), ]
        [TestCategory("41")]
        public void TestStress3b()
        {
            TestStress3();
        }
        [TestMethod(), ]
        [TestCategory("42")]
        public void TestStress3c()
        {
            TestStress3();
        }

        [TestMethod(), ]
        [TestCategory("43")]
        public void TestStress4()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, ("=A1" + (i + 1).ToString()));
            }
            LinkedList<string> firstCells = new LinkedList<string>();
            LinkedList<string> lastCells = new LinkedList<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.AddFirst("A1" + i);
                lastCells.AddFirst("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SequenceEqual(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SequenceEqual(lastCells));
        }
        [TestMethod(), ]
        [TestCategory("44")]
        public void TestStress4a()
        {
            TestStress4();
        }
        [TestMethod(), ]
        [TestCategory("45")]
        public void TestStress4b()
        {
            TestStress4();
        }
        [TestMethod(), ]
        [TestCategory("46")]
        public void TestStress4c()
        {
            TestStress4();
        }

        [TestMethod(), ]
        [TestCategory("47")]
        public void TestStress5()
        {
            RunRandomizedTest(47, 2519);
        }

        [TestMethod(), ]
        [TestCategory("48")]
        public void TestStress6()
        {
            RunRandomizedTest(48, 2521);
        }

        [TestMethod(), ]
        [TestCategory("49")]
        public void TestStress7()
        {
            RunRandomizedTest(49, 2526);
        }

        [TestMethod(), ]
        [TestCategory("50")]
        public void TestStress8()
        {
            RunRandomizedTest(50, 2521);
        }

        /// <summary>
        /// Sets random contents for a random cell 10000 times
        /// </summary>
        /// <param name="seed">Random seed</param>
        /// <param name="size">The known resulting spreadsheet size, given the seed</param>
        public void RunRandomizedTest(int seed, int size)
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        /// <summary>
        /// Generates a random cell name with a capital letter and number between 1 - 99
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        /// <summary>
        /// Generates a random Formula
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }


        #endregion Complicated Tests
    }
}