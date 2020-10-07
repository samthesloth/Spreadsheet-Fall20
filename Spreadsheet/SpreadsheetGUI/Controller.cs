// Author: Sam Peters
//Version 1.0 - 10/5/2020 - Implemented some of the cell features
//Version 1.2 - 10/6/2020 - (Hopefully) finished cell features and added safety closing
//Version 1.3 - 10/7/2020 - Added background worker for enter button, added save and load dialog, added arrow key support


using SS;
using SpreadsheetUtilities;
using System.Windows.Forms;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
        private Spreadsheet sheet = new Spreadsheet(isValid, s => s.ToUpper(), "1.3");
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
        internal void enterButton(SpreadsheetPanel ss, string content, System.ComponentModel.DoWorkEventArgs e)
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

            //Updates cells
            updateCells(sheet, ss, cellsToUpdate);

            //Sets result of worker to return content and value
            e.Result = new Tuple<string, string>(returnContent, returnValue);
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
        /// Opens dialog for user to save their spreadsheet
        /// </summary>
        internal void save()
        {
            //Creates save dialog box
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save Spreadsheet";
            saveDialog.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
            saveDialog.FilterIndex = 1;
            saveDialog.RestoreDirectory = true;
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".sprd";

            // Saves file or shows error message
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sheet.Save(saveDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("Error saving. Please check file name and try again.", "Invalid file", MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// Opens dialog for user to load their spreadsheet
        /// </summary>
        internal void load(SpreadsheetPanel ss)
        {
            //If current sheet has been changed, prompt the user if they are sure
            if (sheet.Changed)
            {
                if (MessageBox.Show("Spreadsheet isn't saved! Are you sure you want to load another one?", "Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            //Clears current sheet
            ss.Clear();

            //Makes open dialog box
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Open Spreadsheet";
            openDialog.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*";
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;

            //Gets file or shows error message
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Load spreadsheet, update cells, then returns current box's content, value, and name to form to display
                    sheet.Load(openDialog.FileName);
                    updateCells(sheet, ss, new List<string>(sheet.GetNamesOfAllNonemptyCells()));
                    ss.GetSelection(out int x, out int y);
                    string cellName = coordsToCell(x, y);
                    object contents = sheet.GetCellContents(cellName);
                    object value = sheet.GetCellValue(cellName);
                    //Gets contents if formula
                    if (contents is Formula)
                    {
                        contents = "=" + contents.ToString();
                    }
                    //Gets value if formula error
                    if (value is FormulaError)
                    {
                        value = "FormulaError";
                    }
                    form.endCellSelect(contents.ToString(), value.ToString(), cellName);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Invalid file", MessageBoxButtons.OK);
                }
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
