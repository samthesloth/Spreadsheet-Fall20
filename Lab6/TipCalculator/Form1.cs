using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void billLabel_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void billInputBox_TextChanged(object sender, EventArgs e)
        {
            if (billInputBox.TextLength != 0 && tipBox.TextLength != 0)
            {
                
            }
        }

        private void tipBox_TextChanged(object sender, EventArgs e)
        {
            if (billInputBox.TextLength != 0 && tipBox.TextLength != 0)
            {
                double tipPercent = Double.Parse(tipBox.Text) / 100.0;
                double tip = (Double.Parse(billInputBox.Text) * tipPercent);
                tipOutputBox.Text = tip.ToString();
                totalOutputBox.Text = (Double.Parse(billInputBox.Text) + tip).ToString();
            };
        }

        private void tipOutputBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void tipBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
    }
}
