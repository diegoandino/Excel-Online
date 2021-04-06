namespace SS
{
    partial class SpreadsheetForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpreadsheetForm));
            this.CellNameBox = new System.Windows.Forms.TextBox();
            this.CellNameLabel = new System.Windows.Forms.Label();
            this.CellValueLabel = new System.Windows.Forms.Label();
            this.CellValueBox = new System.Windows.Forms.TextBox();
            this.CellContentsLabel = new System.Windows.Forms.Label();
            this.CellContentsBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainPanel = new SS.SpreadsheetPanel();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.FileToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // CellNameBox
            // 
            this.CellNameBox.Location = new System.Drawing.Point(92, 46);
            this.CellNameBox.Name = "CellNameBox";
            this.CellNameBox.ReadOnly = true;
            this.CellNameBox.Size = new System.Drawing.Size(100, 20);
            this.CellNameBox.TabIndex = 1;
            // 
            // CellNameLabel
            // 
            this.CellNameLabel.AutoSize = true;
            this.CellNameLabel.Location = new System.Drawing.Point(28, 49);
            this.CellNameLabel.Name = "CellNameLabel";
            this.CellNameLabel.Size = new System.Drawing.Size(58, 13);
            this.CellNameLabel.TabIndex = 2;
            this.CellNameLabel.Text = "Cell Name:";
            // 
            // CellValueLabel
            // 
            this.CellValueLabel.AutoSize = true;
            this.CellValueLabel.Location = new System.Drawing.Point(198, 49);
            this.CellValueLabel.Name = "CellValueLabel";
            this.CellValueLabel.Size = new System.Drawing.Size(37, 13);
            this.CellValueLabel.TabIndex = 3;
            this.CellValueLabel.Text = "Value:";
            // 
            // CellValueBox
            // 
            this.CellValueBox.Location = new System.Drawing.Point(241, 46);
            this.CellValueBox.Name = "CellValueBox";
            this.CellValueBox.ReadOnly = true;
            this.CellValueBox.Size = new System.Drawing.Size(100, 20);
            this.CellValueBox.TabIndex = 4;
            // 
            // CellContentsLabel
            // 
            this.CellContentsLabel.AutoSize = true;
            this.CellContentsLabel.Location = new System.Drawing.Point(347, 49);
            this.CellContentsLabel.Name = "CellContentsLabel";
            this.CellContentsLabel.Size = new System.Drawing.Size(52, 13);
            this.CellContentsLabel.TabIndex = 5;
            this.CellContentsLabel.Text = "Contents:";
            // 
            // CellContentsBox
            // 
            this.CellContentsBox.Location = new System.Drawing.Point(405, 46);
            this.CellContentsBox.Name = "CellContentsBox";
            this.CellContentsBox.Size = new System.Drawing.Size(100, 20);
            this.CellContentsBox.TabIndex = 6;
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowDrop = true;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Location = new System.Drawing.Point(9, 1);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(202, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "File";
            // 
            // FileToolStrip
            // 
            this.FileToolStrip.AllowDrop = true;
            this.FileToolStrip.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.FileToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.FileToolStrip.Location = new System.Drawing.Point(0, 0);
            this.FileToolStrip.Name = "FileToolStrip";
            this.FileToolStrip.Size = new System.Drawing.Size(1146, 25);
            this.FileToolStrip.TabIndex = 8;
            this.FileToolStrip.Text = "FileItem";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.printToolStripMenuItem});
            this.toolStripDropDownButton1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.printToolStripMenuItem.Text = "Print";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPanel.Location = new System.Drawing.Point(0, 75);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1146, 688);
            this.MainPanel.TabIndex = 0;
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.TimeLabel.Location = new System.Drawing.Point(62, 6);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(56, 13);
            this.TimeLabel.TabIndex = 9;
            this.TimeLabel.Text = "TimeLabel";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1146, 764);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.FileToolStrip);
            this.Controls.Add(this.CellContentsBox);
            this.Controls.Add(this.CellContentsLabel);
            this.Controls.Add(this.CellValueBox);
            this.Controls.Add(this.CellValueLabel);
            this.Controls.Add(this.CellNameLabel);
            this.Controls.Add(this.CellNameBox);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpreadsheetForm";
            this.Text = "SpreadsheetForm de_surf_3500";
            this.Load += new System.EventHandler(this.SpreadsheetForm_Load);
            this.FileToolStrip.ResumeLayout(false);
            this.FileToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SpreadsheetPanel MainPanel;
        private System.Windows.Forms.TextBox CellNameBox;
        private System.Windows.Forms.Label CellNameLabel;
        private System.Windows.Forms.Label CellValueLabel;
        private System.Windows.Forms.TextBox CellValueBox;
        private System.Windows.Forms.Label CellContentsLabel;
        private System.Windows.Forms.TextBox CellContentsBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStrip FileToolStrip;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Drawing.Printing.PrintDocument printDocument;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.Timer timer;
    }
}

