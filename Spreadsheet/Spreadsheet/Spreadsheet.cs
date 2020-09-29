// Author: Sam Peters

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    /// <summary>
    /// A spreadsheet consists of an infinite number of named cells.
    ///
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    ///
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    ///
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to
    /// a name, each cell has a contents and a value.  The distinction is important.
    ///
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    ///
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    ///
    /// If a cell's contents is a string, its value is that string.
    ///
    /// If a cell's contents is a double, its value is that double.
    ///
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the
    /// value of the spreadsheet cell it names (if that cell's value is a double) or
    /// is undefined (otherwise).
    ///
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> sheet;
        internal static DependencyGraph graph;
        private string version;
        private string filepath;


        /// <summary>
        /// Creates spreadsheet class, initializing the dictionary for names of cells and the actual cells, as well as the dependency graph to hold the dependencies of cells (by using their names.)
        /// It also assigns the isValid and normalize delegates as well as the version string. 
        /// </summary>
        public Spreadsheet() : this(n => true, n => n, "default")
        {
        }

        /// <summary>
        /// Creates spreadsheet class, initializing the dictionary for names of cells and the actual cells, as well as the dependency graph to hold the dependencies of cells (by using their names.)
        /// It also assigns the isValid and normalize delegates as well as the version string. 
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            Changed = false;

            if (isValid is null)
                IsValid = s => true;
            if (normalize is null)
                Normalize = s => s;

            this.version = version;

            sheet = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
        }

        /// <summary>
        /// Calls the 3-argument constructor while also adding the filepath to the program
        /// </summary>
        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) : this(isValid, normalize, version)
        {
            GetSavedVersion(filepath);
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            //If invalid, throw exception
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z]+[0-9]+$"))
            {
                throw new InvalidNameException();
            }

            //Return contents or return empty string if cell does not exist
            if (sheet.ContainsKey(name))
                return sheet[name].contents;
            else
                return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Return each cell name in sheet, unless content is empty string (aka empty)
            foreach (String s in sheet.Keys)
            {
                if (!sheet[s].contents.Equals(""))
                    yield return s;
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            //Replace dependees with empty list, since a double won't depend on anything
            graph.ReplaceDependees(name, new List<string>());

            //Set cell's contents to number. If nonexistent, make new cell
            if (sheet.ContainsKey(name))
            {
                sheet[name].contents = number;
            }
            else
            {
                sheet.Add(name, new Cell(name, number));
            }

            return new List<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            //Replace dependees with empty list, since text won't depend on anything
            graph.ReplaceDependees(name, new List<string>());

            //Set cell's contents to text. If nonexistent, make new cell
            if (sheet.ContainsKey(name))
            {
                sheet[name].contents = text;
            }
            else
            {
                sheet.Add(name, new Cell(name, text));
            }

            return new List<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        ///
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            //Replace dependees with empty list, since text won't depend on anything. Keeps backup in case of cycle
            List<string> backupDependees = new List<string>(graph.GetDependees(name));
            graph.ReplaceDependees(name, formula.GetVariables());

            //Set cell's contents to text. If nonexistent, make new cell
            object backupContents;
            if (sheet.ContainsKey(name))
            {
                backupContents = sheet[name].contents;
                sheet[name].contents = formula;
            }
            else
            {
                backupContents = "";
                sheet.Add(name, new Cell(name, formula));
            }

            List<string> cellsToRecalculate;
            try
            {
                //Get cells that we need to recalculate (since the next few lines will change these by removing dependencies)
                cellsToRecalculate = new List<string>(GetCellsToRecalculate(name));
            }
            catch
            {
                //If cycle, set values and dependees back to original values, then throw cyclic exception
                sheet[name].contents = backupContents;
                graph.ReplaceDependees(name, backupDependees);
                throw new CircularException();
            }

            return cellsToRecalculate;
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        ///
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return graph.GetDependents(name);
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            Changed = false;

            //Sets up xmlwriter
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlWriter writer = null;

            //Write file
            try
            {
                writer = XmlWriter.Create(filename, settings);

                //Top of document
                writer.WriteStartDocument();
                writer.WriteStartElement("Spreadsheet");
                writer.WriteAttributeString("version", version);

                //Cells
                foreach (string s in GetNamesOfAllNonemptyCells())
                {
                    writer.WriteStartElement("Cell");
                    writer.WriteStartElement("Name");
                    writer.WriteString(s);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Content");
                    if (sheet[s].contents is double)
                        writer.WriteString(sheet[s].contents.ToString());
                    else if (sheet[s].contents is string)
                        writer.WriteString(sheet[s].contents.ToString());
                    else {
                        Formula formulaContents = (Formula)sheet[s].contents;
                        writer.WriteString("=" + formulaContents.ToString());
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                //End
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            catch
            {
                writer.Dispose();
                throw new SpreadsheetReadWriteException("Problem saving xml file. Check if filename is valid");
            }
            finally
            {
                writer.Dispose();
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z]+[0-9]+$") || !IsValid(Normalize(name)) || !Regex.IsMatch(Normalize(name), "^[a-zA-Z]+[0-9]+$"))
                throw new InvalidNameException();
            name = Normalize(name);
            return sheet[name].value;
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            Changed = true;

            //List to be returned
            List<string> list;

            //Check if content isn't null, name is valid, and normalized name is valid. Then normalizes name
            if (content is null)
                throw new ArgumentNullException();
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z]+[0-9]+$") || !IsValid(Normalize(name)) || !Regex.IsMatch(Normalize(name), "^[a-zA-Z]+[0-9]+$"))
                throw new InvalidNameException();
            name = Normalize(name);

            //If content is double, make cell with double
            if (double.TryParse(content, out double outDouble))
                list = new List<string>(SetCellContents(name, outDouble));

            //If content is formula, attempt to make cell with formula
            else if (content[0] == '=')
            {
                Formula f;
                try
                {
                    f = new Formula(content.Substring(1), Normalize, IsValid);
                }
                catch
                {
                    throw new FormulaFormatException("Invalid formula for cell " + name);
                }

                list = new List<string>(SetCellContents(name, f));
            }

            //If content is string, make cell with string
            else
            {
                list = new List<string>(SetCellContents(name, content));
            }

            return list;
        }



        private double lookup(string s)
        {
            return (double)GetCellValue(s);
        }

        private Func<string, double> converted = lookup;


        /// <summary>
        /// Holds the information of each cell in order to be called in the spreadsheet methods. Contains contents and value. For more information, check Spreadsheet class comments
        /// </summary>
        private class Cell
        {
            //Hold name, contents, and value of cell
            public string name;

            public object contents;
            public object value;


            /// <summary>
            /// Builds cell with designated contents, and then sets value according to what type the content is.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="content"></param>
            public Cell(string name, object content)
            {
                //We don't check for if name is valid b/c set cell methods do this for us

                //Sets name and content. Then finds value based on type of content
                this.name = name;
                contents = content;
                value = 0;
                if (contents is string)
                {
                    value = contents;
                }

                else if (contents is int)
                {
                    value = contents;
                }

                else if (contents is Formula)
                {
                    Formula f = (Formula)contents;
                    value = f.Evaluate(lookup);
                    foreach (string s in f.GetVariables())
                    {
                        graph.AddDependency(s, name);
                    }
                }
            }
        }
    }
}