using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public delegate void enterButtonClickHandler(SpreadsheetPanel panel, string content);

    public partial class SheetForm : Form
    {
        public event enterButtonClickHandler enterButtonClick;

        public SheetForm()
        {
            InitializeComponent();
            Controller control = new Controller(this);

            spreadsheetPanel.SelectionChanged += control.cellSelect;
            enterButtonClick += control.enterButton;
        }

        // Deals with the New menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            SheetFormAppContext.getAppContext().RunForm(new SheetForm());
        }

        // Deals with the Close menu
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        internal void endCellSelect(string content, string value)
        {
            contentBox.Text = content;
            valueBox.Text = value;
            contentBox.Focus();
        }

        private void contentBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void enterButton_Click(object sender, EventArgs e)
        {
            enterButtonClick(spreadsheetPanel, contentBox.Text);
        }

        internal void enterButtonEnd(string content, string value)
        {
            contentBox.Text = content;
            valueBox.Text = value;
            spreadsheetPanel.GetSelection(out int x, out int y);
            spreadsheetPanel.SetValue(x, y, value);
        }
    }
}
