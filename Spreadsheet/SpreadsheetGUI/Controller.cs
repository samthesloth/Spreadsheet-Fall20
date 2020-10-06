using SS;
using SpreadsheetUtilities;
using System.Windows.Forms;

namespace SpreadsheetGUI
{

    internal class Controller
    {
        //Spreadsheet to hold cells of spreadsheet
        private Spreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");
        //Form to be used
        private SheetForm form;

        public Controller(SheetForm form)
        {
            this.form = form;
        }

        internal void cellSelect(SpreadsheetPanel ss)
        {
            ss.GetSelection(out int x, out int y);
            string cellName = coordsToCell(x, y);
            object contents = sheet.GetCellContents(cellName);
            if(contents is Formula)
            {
                contents = "=" + contents.ToString();
            }
            form.endCellSelect(contents.ToString(), sheet.GetCellValue(cellName).ToString());
        }

        internal void enterButton(SpreadsheetPanel ss, string content)
        {
            //Get cell name
            ss.GetSelection(out int x, out int y);
            string cellName = coordsToCell(x, y);
            //Try to set contents of cell. If unsuccessful, display error message
            try
            {
                sheet.SetContentsOfCell(cellName, content);
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

            //Get value into a string to be passed to form
            string returnValue;
            object tempValue = sheet.GetCellValue(cellName);
            if (tempValue is FormulaError)
                returnValue = "FormulaError";
            else
                returnValue = tempValue.ToString();

            form.enterButtonEnd(returnContent, returnValue);
        }

        public string coordsToCell(int x, int y)
        {
            char letter = (char)('A' + x);
            int number = y + 1;
            return "" + letter + number;
        }
    }
}
