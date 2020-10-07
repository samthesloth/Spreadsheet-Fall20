// Author: Sam Peters
//Version 1.0 - 10/4/2020 - Set up project, began writing cell features
//Version 1.1 - 10/5/2020 - Implemented more of the cell features
//Version 1.2 - 10/6/2020 - (Hopefully) finished cell features and added safety closing
//Version 1.3 - 10/7/2020 - Added background worker for enter button, added save and load dialog, added arrow key support


using SS;
using System;
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
            //Initialize
            InitializeComponent();

            //Creates controller
            control = new Controller(this);

            //Sets up tooltips
            contentTip = new ToolTip();
            contentTip.SetToolTip(contentBox, contentBox.Text);
            valueTip = new ToolTip();
            valueTip.SetToolTip(valueBox, valueBox.Text);

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
            // thread as the other forms.
            SheetFormAppContext.getAppContext().RunForm(new SheetForm());
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


        private void contentBox_MouseEnter(object sender, EventArgs e)
        {
            contentTip.Show(contentBox.Text, this);
        }

        //Handles when user pushed an arrow key, and then moves a cell over accordingly
        private void SheetForm_KeyDown(object sender, KeyEventArgs e)
        {
            spreadsheetPanel.GetSelection(out int x, out int y);
            switch (e.KeyData)
            {
                case (Keys.Up):
                    //if (y > 0)
                        spreadsheetPanel.SetSelection(x, y - 1);
                    break;
                case (Keys.Down):
                    //if (y < 98)
                        spreadsheetPanel.SetSelection(x, y + 1);
                    break;
                case (Keys.Left):
                    //if (x > 0)
                        spreadsheetPanel.SetSelection(x-1, y);
                    break;
                case (Keys.Right):
                    //if (x < 25)
                        spreadsheetPanel.SetSelection(x+1, y);
                    break;
                default: break;
            }
            control.cellSelect(spreadsheetPanel);
        }
    }
}
