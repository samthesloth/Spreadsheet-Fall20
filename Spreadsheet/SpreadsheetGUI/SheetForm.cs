// Author: Sam Peters
//Version 1.0 - 10/4/2020 - Set up project, began writing cell features
//Version 1.1 - 10/5/2020 - Implemented more of the cell features
//Version 1.2 - 10/6/2020 - (Hopefully) finished cell features and added safety closing
//Version 1.3 - 10/7/2020 - Added background worker for enter button, added save and load dialog, added arrow key support
//Version 1.4 - 10/8/2020 - Added Ctrl functions, help menu, fixed arrow keys
//Version 1.5 - 10/9/2020 - Changed New so it would do the correct behavior, added tooltips for value and content, added ability to go to a cell

using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SpreadsheetGUI
{

    /// <summary>
    /// Form class to hold info on the form of the spreadsheet class
    /// </summary>
    public partial class SheetForm : Form
    {

        //Controller of this form
        Controller control;

        //Sets up tool tips
        ToolTip contentTip;
        ToolTip valueTip;

        /// <summary>
        /// Constructor that created form and controller, as well as sets up events
        /// </summary>
        public SheetForm()
        {
            //Initialize, set first focus to contentBox, and ensure arrow keys work in the program by setting KeyPreview to true
            InitializeComponent();
            this.ActiveControl = contentBox;
            this.KeyPreview = true;

            //Creates controller
            control = new Controller(this);

            //Adds events
            spreadsheetPanel.SelectionChanged += control.cellSelect;

            //Sets up background worker
            calculateWorker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Deals with new menu
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms. Then prompt user to find file to save.
            SheetForm temp = new SheetForm();
            SheetFormAppContext.getAppContext().RunForm(temp);
            temp.control.newSpreadsheet();

        }

        /// <summary>
        /// Deals with close menu
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Deals with closing through other means
        /// </summary>
        private void SheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            control.Close(e);
        }

        /// <summary>
        /// After cell has been selected, shows content and value of cell in respective boxed. Also focuses on contentBox
        /// </summary>
        internal void endCellSelect(string content, string value, string cellName)
        {
            contentBox.Text = content;
            valueBox.Text = value;
            cellBox.Text = cellName;
            contentBox.Focus();
            contentBox.SelectAll();
        }

        /// <summary>
        /// Sets content box and value box to designated values; disables enter button
        /// </summary>
        internal void endEnterButton(string content, string value)
        {
            contentBox.Text = content;
            valueBox.Text = value;
            enterButton.Enabled = true;
        }

        /// <summary>
        /// When enter button is clicked, calls event for controller to disable enter button, change contentBox, and run worker
        /// </summary>
        private void enterButton_Click(object sender, EventArgs e)
        {
            enterButton.Enabled = false;
            string text = contentBox.Text;
            calculateWorker.RunWorkerAsync(text);
        }

        /// <summary>
        /// Starts the control's enter button method
        /// </summary>
        private void calculateWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            control.enterButton(spreadsheetPanel, (string)e.Argument, e);
        }

        /// <summary>
        /// Passes return content and value to end enter button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calculateWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Tuple<string, string> result = (Tuple<string, string>)e.Result;
            endEnterButton(result.Item1, result.Item2);
        }

        /// <summary>
        /// Calls control's method for saving
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            control.save();
        }

        /// <summary>
        /// Calls control's method for loading
        /// </summary>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            control.load(spreadsheetPanel);
        }

        /// <summary>
        /// Handles when user pushed an arrow key, and then moves a cell over accordingly OR 
        /// Handles when user does ctrl functions
        /// </summary>
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+key
            if(e.Control)
            {
                switch(e.KeyCode)
                {
                    //+s to save
                    case (Keys.S):
                        control.save();
                        break;
                    //+n for new sheet
                    case (Keys.N):
                        SheetForm temp = new SheetForm();
                        SheetFormAppContext.getAppContext().RunForm(temp);
                        temp.control.newSpreadsheet();
                        break;
                    //+o for open sheet
                    case (Keys.O):
                        control.load(spreadsheetPanel);
                        break;
                    //+w to close
                    case (Keys.W):
                        Close();
                        break;
                    //Return with default so final bit doesn't get called
                    default:
                        return;
                }
                //Prevents windows ding
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            //Change selection based on which key is pushed
            spreadsheetPanel.GetSelection(out int x, out int y);
            switch (e.KeyData)
            {
                case (Keys.Up):
                    spreadsheetPanel.SetSelection(x, y - 1);
                    control.cellSelect(spreadsheetPanel);
                    break;
                case (Keys.Down):
                    spreadsheetPanel.SetSelection(x, y + 1);
                    control.cellSelect(spreadsheetPanel);
                    break;
                case (Keys.Left):
                    spreadsheetPanel.SetSelection(x-1, y);
                    control.cellSelect(spreadsheetPanel);
                    break;
                case (Keys.Right):
                    spreadsheetPanel.SetSelection(x+1, y);
                    control.cellSelect(spreadsheetPanel);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Displays help message when help is clicked
        /// </summary>
        private void helpMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select a cell and type into the content box and push evaluate or enter to put it into the cell.\n\n" +
                "Use the arrow keys to move around the cells and the mouse wheel to scroll.\n\n" +
                "You can also go to a cell by typing its name into the Cell text box and clicking 'Find'\n\n" +
                "To make a new sheet (Ctrl+N), save your sheet (Ctrl+S), load a sheet (Ctrl+O), or close the current sheet (Ctrl+W), push 'File' in the top left.\n\n" +
                "If the contents or value of a cell is too large to see, you can hover over the respected boxes to see the whole entry.\n", "Spreadsheet Help", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Opens README when readme button is clicked
        /// </summary>
        private void readMeMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"..\..\..\README.txt");
        }

        /// <summary>
        /// Displays tooltip of contents when hovering over content box
        /// </summary>
        private void contentBox_MouseEnter(object sender, EventArgs e)
        {
            contentTip = new ToolTip();
            contentTip.InitialDelay = 0;
            contentTip.Show(contentBox.Text, contentBox);
        }

        /// <summary>
        /// Removes tooltip when mouse leaves content box
        /// </summary>
        private void contentBox_MouseLeave(object sender, EventArgs e)
        {
            contentTip.Dispose();
        }

        /// <summary>
        /// Displays tooltip of value when hovering over value box
        /// </summary>
        private void valueBox_MouseEnter(object sender, EventArgs e)
        {
            valueTip = new ToolTip();
            valueTip.InitialDelay = 0;
            valueTip.Show(valueBox.Text, valueBox);
        }

        /// <summary>
        /// Removes tooltip when mouse leaves value box
        /// </summary>
        private void valueBox_MouseLeave(object sender, EventArgs e)
        {
            valueTip.Dispose();
        }

        /// <summary>
        /// When focus is on cell box, make find button visible to click on
        /// </summary>
        private void cellBox_Enter(object sender, EventArgs e)
        {
            findButton.Visible = true;
        }

        /// <summary>
        /// If focus is taken off cell box (and find button), make find button invisible and reselect cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cellBox_Leave(object sender, EventArgs e)
        {
            if (!findButton.Focused)
            {
                findButton.Visible = false;
                control.cellSelect(spreadsheetPanel);
            }
        }

        /// <summary>
        /// When the find button is clicked, go to that cell. If not valid, show message and go to A1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findButton_Click(object sender, EventArgs e)
        {
            //If cell name matches, go to cell, and display contents and value
            if (Regex.IsMatch(cellBox.Text, "^[A-Za-z][1-9][0-9]?$"))
            {
                cellBox.Text = cellBox.Text.ToUpper();
                Controller.cellToCoords(cellBox.Text, out int x, out int y);
                spreadsheetPanel.SetSelection(x, y);
            }
            //Otherwise, select A1 and show an error message
            else
            {
                spreadsheetPanel.SetSelection(0, 0);
                control.cellSelect(spreadsheetPanel);
                MessageBox.Show("Cell name is not valid. Please try again", "Invalid cell", MessageBoxButtons.OK);
            }
            //Make find button invisible
            findButton.Visible = false;
        }
    }
}
