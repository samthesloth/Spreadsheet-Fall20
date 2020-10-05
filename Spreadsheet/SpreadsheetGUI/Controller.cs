using SS;

namespace SpreadsheetGUI
{

    internal class Controller
    {
        //Spreadsheet to hold cells of spreadsheet
        private Spreadsheet sheet = new Spreadsheet();

        public void cellSelect(SpreadsheetPanel ss)
        {
            ss.GetSelection(out int x, out int y);
            string cellName = coordsToCell(x, y);
            SheetForm.newContent(x, y, (string)sheet.GetCellContents(cellName));
        }

        public string coordsToCell(int x, int y)
        {
            char letter = (char)('A' + x);
            int number = y + 1;
            return "" + letter + number;
        }
    }
}
