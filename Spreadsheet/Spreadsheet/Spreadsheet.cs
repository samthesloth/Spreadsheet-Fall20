// Author: Sam Peters

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Creates spreadsheet class, initializing the dictionary for names of cells and the actual cells, as well as the dependency graph to hold the dependencies of cells (by using their names.)
        /// </summary>
        public Spreadsheet()
        {
            sheet = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            //If invalid, throw exception
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
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
            foreach(String s in sheet.Keys)
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
        public override IList<string> SetCellContents(string name, double number)
        {
            //Check if name is valid. Throws exception if not
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
            {
                throw new InvalidNameException();
            }

            //Get cells that we need to recalculate (since the next few lines will change these by removing dependencies)
            List<string> cellsToRecalculate = new List<string>(GetCellsToRecalculate(name));
            //Replace dependees with empty list, since a double won't depend on anything
            graph.ReplaceDependees(name, new List<string>());

            //Set cell's contents to number. If nonexistent, make new cell
            if (sheet.ContainsKey(name))
                sheet[name].contents = number;
            else
                sheet.Add(name, new Cell(name, number));

            return cellsToRecalculate;
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
        public override IList<string> SetCellContents(string name, string text)
        {
            //Check if text and name are valid. Throws exception if not
            if (text is null)
                throw new ArgumentNullException();
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
            {
                throw new InvalidNameException();
            }

            //Get cells that we need to recalculate (since the next few lines will change these by removing dependencies)
            List<string> cellsToRecalculate = new List<string>(GetCellsToRecalculate(name));
            //Replace dependees with empty list, since a string won't depend on anything
            graph.ReplaceDependees(name, new List<string>());

            //Set cell's contents to string. If nonexistent, make new cell
            if (sheet.ContainsKey(name))
                sheet[name].contents = text;
            else
                sheet.Add(name, new Cell(name, text));

            return cellsToRecalculate;
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
        public override IList<string> SetCellContents(string name, Formula formula)
        {
            //Check if formula and name are valid. Throws exception if not
            if (formula is null)
                throw new ArgumentNullException();
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
            {
                throw new InvalidNameException();
            }

            //Get cells that we need to recalculate (since the next few lines will change these by removing dependencies)
            List<string> cellsToRecalculate = new List<string>(GetCellsToRecalculate(name));
            //Replace dependees with empty list, as to remove them to have new one's added later
            graph.ReplaceDependees(name, new List<string>());

            //Set cell's contents to formula. If nonexistent, make new cell
            if (sheet.ContainsKey(name))
                sheet[name].contents = formula;
            else
                sheet.Add(name, new Cell(name, formula));

            //Add new dependees with new formula's variables
            List<string> newDependees = new List<string>(formula.GetVariables());
            graph.ReplaceDependees(name, newDependees);

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
        /// Holds the information of each cell in order to be called in the spreadsheet methods. Contains contents and value. For more information, check Spreadsheet class comments
        /// </summary>
        private class Cell{
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
                //Check if name is valid. If not, throws exception
                if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
                {
                    throw new InvalidNameException();
                }

                //Sets name and content. Then finds value based on type of content
                this.name = name;
                contents = content;
                if (contents is string)
                {
                    value = contents;
                }

                else if(contents is int)
                {
                    value = contents;
                }

                else if(contents is Formula)
                {
                    Formula f = (Formula)contents;
                    value = f.Evaluate(s=>1);
                    foreach(string s in f.GetVariables())
                    {
                        graph.AddDependency(s, name);
                    }
                }
            }

            /// <summary>
            /// Returns whether this cell is equal to o. If o is null or not a cell, returns false. Otherwise, returns true if the cells have the same name, contents, and value
            /// </summary>
            /// <param name="o">Object to be compared</param>
            /// <returns>Whether they are equal or not</returns>
            public override bool Equals(object o)
            {
                if (o is null || !(o is Cell))
                    return false;
                Cell other = (Cell)o;
                return (name == other.name && contents == other.contents && value == other.value);
            }

            /// <summary>
            /// Returns whether the cells are equal, checking for null values as well. 
            /// </summary>
            /// <param name="c1">First item to be compared</param>
            /// <param name="c2">Second item to be compared</param>
            /// <returns></returns>
            public static bool operator ==(Cell c1, Cell c2)
            {
                if (c1 is null && c2 is null)
                    return true;
                if ((c1 is null && !(c2 is null)) || (c2 is null && !(c1 is null)))
                    return false;
                return c1.Equals(c2);
            }

            /// <summary>
            /// Returns whether the cells are not equal, checking for null values as well. Calls the opposite of ==
            /// </summary>
            /// <param name="c1">First cell to be compared</param>
            /// <param name="c2">Second cell to be compared</param>
            /// <returns>Whether they are inequal or not</returns>
            public static bool operator !=(Cell c1, Cell c2)
            {
                return !(c1 == c2);
            }

            /// <summary>
            /// Returns the hashcode of the cell, which is the added hash codes of the name, contents, and value of the cell. 
            /// </summary>
            /// <returns>Hashcode of cell</returns>
            public override int GetHashCode()
            {
                return name.GetHashCode() + contents.GetHashCode() + value.GetHashCode();
            }
        }
    }
}
