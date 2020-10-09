//Author - Sam Peters

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
            this.contentLabel = new System.Windows.Forms.Label();
            this.valueLabel = new System.Windows.Forms.Label();
            this.cellLabel = new System.Windows.Forms.Label();
            this.cellBox = new System.Windows.Forms.TextBox();
            this.calculateWorker = new System.ComponentModel.BackgroundWorker();
            this.spreadsheetPanel = new SS.SpreadsheetPanel();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentBox
            // 
            this.contentBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.contentBox.ForeColor = System.Drawing.SystemColors.Window;
            this.contentBox.Location = new System.Drawing.Point(12, 190);
            this.contentBox.Name = "contentBox";
            this.contentBox.Size = new System.Drawing.Size(108, 22);
            this.contentBox.TabIndex = 1;
            this.contentBox.MouseEnter += new System.EventHandler(this.contentBox_MouseEnter);
            // 
            // valueBox
            // 
            this.valueBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.valueBox.ForeColor = System.Drawing.SystemColors.Window;
            this.valueBox.Location = new System.Drawing.Point(12, 299);
            this.valueBox.Name = "valueBox";
            this.valueBox.ReadOnly = true;
            this.valueBox.Size = new System.Drawing.Size(108, 22);
            this.valueBox.TabIndex = 2;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ControlText;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
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
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Window;
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
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
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
            this.enterButton.ForeColor = System.Drawing.SystemColors.Window;
            this.enterButton.Location = new System.Drawing.Point(21, 217);
            this.enterButton.Name = "enterButton";
            this.enterButton.Size = new System.Drawing.Size(90, 26);
            this.enterButton.TabIndex = 4;
            this.enterButton.Text = "Evalutate";
            this.enterButton.UseVisualStyleBackColor = false;
            this.enterButton.Click += new System.EventHandler(this.enterButton_Click);
            // 
            // contentLabel
            // 
            this.contentLabel.AutoSize = true;
            this.contentLabel.BackColor = System.Drawing.SystemColors.MenuText;
            this.contentLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.contentLabel.Location = new System.Drawing.Point(35, 166);
            this.contentLabel.Name = "contentLabel";
            this.contentLabel.Size = new System.Drawing.Size(64, 17);
            this.contentLabel.TabIndex = 5;
            this.contentLabel.Text = "Contents";
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.BackColor = System.Drawing.SystemColors.MenuText;
            this.valueLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.valueLabel.Location = new System.Drawing.Point(45, 277);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(44, 17);
            this.valueLabel.TabIndex = 6;
            this.valueLabel.Text = "Value";
            // 
            // cellLabel
            // 
            this.cellLabel.AutoSize = true;
            this.cellLabel.BackColor = System.Drawing.SystemColors.MenuText;
            this.cellLabel.ForeColor = System.Drawing.SystemColors.Window;
            this.cellLabel.Location = new System.Drawing.Point(45, 85);
            this.cellLabel.Name = "cellLabel";
            this.cellLabel.Size = new System.Drawing.Size(31, 17);
            this.cellLabel.TabIndex = 8;
            this.cellLabel.Text = "Cell";
            // 
            // cellBox
            // 
            this.cellBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.cellBox.ForeColor = System.Drawing.SystemColors.Window;
            this.cellBox.Location = new System.Drawing.Point(12, 110);
            this.cellBox.Name = "cellBox";
            this.cellBox.ReadOnly = true;
            this.cellBox.Size = new System.Drawing.Size(108, 22);
            this.cellBox.TabIndex = 7;
            // 
            // calculateWorker
            // 
            this.calculateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.calculateWorker_DoWork);
            this.calculateWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.calculateWorker_RunWorkerCompleted);
            // 
            // spreadsheetPanel
            // 
            this.spreadsheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.spreadsheetPanel.Location = new System.Drawing.Point(126, 31);
            this.spreadsheetPanel.Name = "spreadsheetPanel";
            this.spreadsheetPanel.Size = new System.Drawing.Size(747, 441);
            this.spreadsheetPanel.TabIndex = 0;
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Menu;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // SheetForm
            // 
            this.AcceptButton = this.enterButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(885, 484);
            this.Controls.Add(this.cellLabel);
            this.Controls.Add(this.cellBox);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.contentLabel);
            this.Controls.Add(this.enterButton);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.contentBox);
            this.Controls.Add(this.spreadsheetPanel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "SheetForm";
            this.Text = "Spreadsheet";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
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
        private System.Windows.Forms.Label contentLabel;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label cellLabel;
        private System.Windows.Forms.TextBox cellBox;
        private System.ComponentModel.BackgroundWorker calculateWorker;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    }
}

