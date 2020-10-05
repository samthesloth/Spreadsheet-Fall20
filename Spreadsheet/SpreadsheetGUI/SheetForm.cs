using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class SheetForm : Form
    {

        public SheetForm()
        {
            InitializeComponent();
            Controller control = new Controller();

            spreadsheetPanel.SelectionChanged += control.cellSelect;
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

        internal static void newContent(int x, int y, string content)
        {

        } 
    }
}
