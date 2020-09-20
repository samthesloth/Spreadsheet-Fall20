// Author: Sam Peters

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
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
        /// TODO : what does constructor do?
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
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
            {
                throw new InvalidNameException();
            }

            return sheet[name].contents;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
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
            if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
            {
                throw new InvalidNameException();
            }

            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
                if (name is null || !Regex.IsMatch(name, "^[a-zA-Z_]+[0-9a-zA-Z_]*$"))
                {
                    throw new InvalidNameException();
                }

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
                    value = f.Evaluate(null);
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
                if (name == other.name && contents == other.contents && value == other.value)
                    return true;
                return false;
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
