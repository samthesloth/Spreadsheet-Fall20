// Author: Sam Peters
//Version 1.0 - 10/5/2020 - Implemented some of the cell features
//Version 1.2 - 10/6/2020 - (Hopefully) finished cell features and added safety closing


using SS;
using SpreadsheetUtilities;
using System.Windows.Forms;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Holds methods that are called through events and directly in order to communicate between the form and the spreadsheet
    /// </summary>
    internal class Controller
    {
        //Delegate for cell validity
        private static Func<string, bool> isValid = validCellName;

        //Spreadsheet to hold cells of spreadsheet
        private Spreadsheet sheet = new Spreadsheet(isValid, s => s.ToUpper(), "default");
        //Form to be used
        private SheetForm form;

        /// <summary>
        /// Created controller that has access to form
        /// </summary>
        public Controller(SheetForm form)
        {
            this.form = form;
        }

        /// <summary>
        /// Checks if the file has been changed before closing. Shows message if it has
        /// </summary>
        public void Close(FormClosingEventArgs e)
        {
            //If sheet has been changed, ask the user if they are sure they want to close the spreadsheet
            if (sheet.Changed)
            {
                if (MessageBox.Show("Spreadsheet isn't saved! Are you sure you want to quit?", "Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// When cell is selected, get contents and val to put in text boxes
        /// </summary>
        public void cellSelect(SpreadsheetPanel ss)
        {
            //Gets cell name
            ss.GetSelection(out int x, out int y);
            string cellName = coordsToCell(x, y);
            //Gets contents and value
            object contents = sheet.GetCellContents(cellName);
            object value = sheet.GetCellValue(cellName);
            //Gets contents if formula
            if(contents is Formula)
            {
                contents = "=" + contents.ToString();
            }
            //Gets value if formula error
            if(value is FormulaError)
            {
                value = "FormulaError";
            }
            //Returns contents and value
            form.endCellSelect(contents.ToString(), value.ToString(), cellName);
        }

        /// <summary>
        /// When enter button is pushed, calculates new content for cell and return value
        /// </summary>
        internal void enterButton(SpreadsheetPanel ss, string content)
        {
            //List to store cells that we need to update
            List<string> cellsToUpdate = new List<string>();

            //Get cell name
            ss.GetSelection(out int x, out int y);
            string cellName = coordsToCell(x, y);
            //Try to set contents of cell. If unsuccessful, display error message
            try
            {
                cellsToUpdate = new List<string>(sheet.SetContentsOfCell(cellName, content));
            }
            catch(FormulaFormatException)
            {
                MessageBox.Show("Invalid formula entered!", "Invalid content", MessageBoxButtons.OK);
            }
            catch(CircularException)
            {
                MessageBox.Show("Circular exception found!", "Invalid content", MessageBoxButtons.OK);
            }

            //Get content into a string to be passed to form
            string returnContent;
            object tempContent = sheet.GetCellContents(cellName);
            if (tempContent is Formula)
                returnContent = "=" + tempContent.ToString();
            else
                returnContent = tempContent.ToString();

            //Get value into a string to be passed to updateCells
            string returnValue;
            object tempValue = sheet.GetCellValue(cellName);
            if (tempValue is FormulaError)
                returnValue = "FormulaError";
            else
                returnValue = tempValue.ToString();

            //Makes a thread to update the cells of the panel
            Thread cellUpdate = new Thread(() => updateCells(sheet, ss, cellsToUpdate));
            cellUpdate.Start();

            //Returns content and value to form to display
            form.endEnterButton(returnContent, returnValue);
        }

        /// <summary>
        /// Updates cell values in the spreadsheet panel
        /// </summary>
        public static void updateCells(Spreadsheet sheet, SpreadsheetPanel ss, List<string> cells)
        {
            object contents;
            object value;
            foreach (string cellName in cells)
            {
                contents = sheet.GetCellContents(cellName);
                value = sheet.GetCellValue(cellName);
                //Gets contents if formula
                if (contents is Formula)
                {
                    contents = "=" + contents.ToString();
                }
                //Gets value if formula error
                if(value is FormulaError)
                {
                    value = "FormulaError";
                }
                //Updates cell values and returns contents and value
                cellToCoords(cellName, out int x, out int y);
                ss.SetValue(x, y, value.ToString());
            }
        }

        /// <summary>
        /// Helper method that gets the cell name from x and y coordinates
        /// </summary>
        internal static string coordsToCell(int x, int y)
        {
            char letter = (char)('A' + x);
            int number = y + 1;
            return "" + letter + number;
        }

        /// <summary>
        /// Helper method that gets the coordinates of the cell from the name
        /// </summary>
        internal static void cellToCoords(string s, out int x, out int y)
        {
            x = s[0] - 65;
            y = int.Parse(s.Substring(1)) - 1;
        }

        /// <summary>
        /// Delegate to be used to ensure cell anmes are valid to our spreadsheet
        /// </summary>
        private static bool validCellName(string s)
        {
            return Regex.IsMatch(s, "^[A-Za-z][1-9][0-9]?$");
        }
    }
}
