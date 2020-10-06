using SS;

namespace SpreadsheetGUI
{
    partial class SheetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spreadsheetPanel = new SS.SpreadsheetPanel();
            this.contentBox = new System.Windows.Forms.TextBox();
            this.valueBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.enterButton = new System.Windows.Forms.Button();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel
            // 
            this.spreadsheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.spreadsheetPanel.Location = new System.Drawing.Point(12, 59);
            this.spreadsheetPanel.Name = "spreadsheetPanel";
            this.spreadsheetPanel.Size = new System.Drawing.Size(861, 413);
            this.spreadsheetPanel.TabIndex = 0;
            // 
            // contentBox
            // 
            this.contentBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.contentBox.ForeColor = System.Drawing.SystemColors.Menu;
            this.contentBox.Location = new System.Drawing.Point(27, 31);
            this.contentBox.Name = "contentBox";
            this.contentBox.Size = new System.Drawing.Size(108, 22);
            this.contentBox.TabIndex = 1;
            this.contentBox.TextChanged += new System.EventHandler(this.contentBox_TextChanged);
            // 
            // valueBox
            // 
            this.valueBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.valueBox.Enabled = false;
            this.valueBox.ForeColor = System.Drawing.SystemColors.Menu;
            this.valueBox.Location = new System.Drawing.Point(269, 31);
            this.valueBox.Name = "valueBox";
            this.valueBox.Size = new System.Drawing.Size(108, 22);
            this.valueBox.TabIndex = 2;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ControlText;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(885, 28);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // enterButton
            // 
            this.enterButton.BackColor = System.Drawing.SystemColors.MenuText;
            this.enterButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.enterButton.ForeColor = System.Drawing.SystemColors.Control;
            this.enterButton.Location = new System.Drawing.Point(141, 31);
            this.enterButton.Name = "enterButton";
            this.enterButton.Size = new System.Drawing.Size(30, 22);
            this.enterButton.TabIndex = 4;
            this.enterButton.UseVisualStyleBackColor = false;
            this.enterButton.Click += new System.EventHandler(this.enterButton_Click);
            // 
            // SheetForm
            // 
            this.AcceptButton = this.enterButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(885, 484);
            this.Controls.Add(this.enterButton);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.contentBox);
            this.Controls.Add(this.spreadsheetPanel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "SheetForm";
            this.Text = "Spreadsheet";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel;
        private System.Windows.Forms.TextBox contentBox;
        private System.Windows.Forms.TextBox valueBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Button enterButton;
    }
}

