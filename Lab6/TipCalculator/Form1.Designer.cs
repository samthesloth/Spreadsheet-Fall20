namespace Lab06
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.billLabel = new System.Windows.Forms.Label();
            this.billInputBox = new System.Windows.Forms.TextBox();
            this.tipLabel = new System.Windows.Forms.Label();
            this.tipBox = new System.Windows.Forms.TextBox();
            this.totalOutputBox = new System.Windows.Forms.TextBox();
            this.tipOutputBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // billLabel
            // 
            this.billLabel.AutoSize = true;
            this.billLabel.BackColor = System.Drawing.SystemColors.ControlText;
            this.billLabel.Font = new System.Drawing.Font("Dubai", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.billLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.billLabel.Location = new System.Drawing.Point(28, 25);
            this.billLabel.Name = "billLabel";
            this.billLabel.Size = new System.Drawing.Size(90, 27);
            this.billLabel.TabIndex = 0;
            this.billLabel.Text = "Bill Amount";
            this.billLabel.Click += new System.EventHandler(this.billLabel_Click);
            // 
            // billInputBox
            // 
            this.billInputBox.Location = new System.Drawing.Point(139, 28);
            this.billInputBox.Name = "billInputBox";
            this.billInputBox.Size = new System.Drawing.Size(100, 23);
            this.billInputBox.TabIndex = 1;
            this.billInputBox.TextChanged += new System.EventHandler(this.billInputBox_TextChanged);
            // 
            // tipLabel
            // 
            this.tipLabel.AutoSize = true;
            this.tipLabel.Font = new System.Drawing.Font("Dubai", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tipLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.tipLabel.Location = new System.Drawing.Point(42, 99);
            this.tipLabel.Name = "tipLabel";
            this.tipLabel.Size = new System.Drawing.Size(52, 27);
            this.tipLabel.TabIndex = 4;
            this.tipLabel.Text = "Tip %";
            this.tipLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // tipBox
            // 
            this.tipBox.Location = new System.Drawing.Point(139, 103);
            this.tipBox.Name = "tipBox";
            this.tipBox.Size = new System.Drawing.Size(100, 23);
            this.tipBox.TabIndex = 2;
            this.tipBox.TextChanged += new System.EventHandler(this.tipBox_TextChanged);
            this.tipBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tipBox_KeyPress);
            // 
            // totalOutputBox
            // 
            this.totalOutputBox.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.totalOutputBox.Location = new System.Drawing.Point(139, 215);
            this.totalOutputBox.Name = "totalOutputBox";
            this.totalOutputBox.ReadOnly = true;
            this.totalOutputBox.Size = new System.Drawing.Size(100, 23);
            this.totalOutputBox.TabIndex = 5;
            this.totalOutputBox.Text = "Total";
            this.totalOutputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tipOutputBox
            // 
            this.tipOutputBox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tipOutputBox.Location = new System.Drawing.Point(139, 173);
            this.tipOutputBox.Name = "tipOutputBox";
            this.tipOutputBox.ReadOnly = true;
            this.tipOutputBox.Size = new System.Drawing.Size(100, 23);
            this.tipOutputBox.TabIndex = 4;
            this.tipOutputBox.Text = "Tip";
            this.tipOutputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tipOutputBox.TextChanged += new System.EventHandler(this.tipOutputBox_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(330, 310);
            this.Controls.Add(this.tipOutputBox);
            this.Controls.Add(this.billInputBox);
            this.Controls.Add(this.tipBox);
            this.Controls.Add(this.totalOutputBox);
            this.Controls.Add(this.tipLabel);
            this.Controls.Add(this.billLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label billLabel;
        private System.Windows.Forms.TextBox billInputBox;
        private System.Windows.Forms.Label tipLabel;
        private System.Windows.Forms.TextBox tipBox;
        private System.Windows.Forms.TextBox totalOutputBox;
        private System.Windows.Forms.TextBox tipOutputBox;
    }
}

