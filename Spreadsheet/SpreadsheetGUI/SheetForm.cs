// Author: Sam Peters
//Version 1.0 - 10/4/2020 - Set up project, began writing cell features
//Version 1.1 - 10/5/2020 - Implemented more of the cell features
//Version 1.2 - 10/6/2020 - (Hopefully) finished cell features and added safety closing


using SS;
using System;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    //Delegate for enter button, that sends panel and new content to controller
    public delegate void enterButtonClickHandler(SpreadsheetPanel panel, string content);

    /// <summary>
    /// Form class to hold info on the form of the spreadsheet class
    /// </summary>
    public partial class SheetForm : Form
    {
        //Event for when the enter button is clicked
        public event enterButtonClickHandler enterButtonClick;

        //Controller of this form
        Controller control;

        /// <summary>
        /// Constructor that created form and controller, as well as sets up events
        /// </summary>
        public SheetForm()
        {
            InitializeComponent();
            control = new Controller(this);

            spreadsheetPanel.SelectionChanged += control.cellSelect;
            enterButtonClick += control.enterButton;
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

        internal void endEnterButton(string content, string value)
        {
            contentBox.Text = content;
            valueBox.Text = value;
        }

        /// <summary>
        /// When enter button is clicked, calls event for controller to do its thing
        /// </summary>
        private void enterButton_Click(object sender, EventArgs e)
        {
            enterButtonClick(spreadsheetPanel, contentBox.Text);
        }
    }
}
