﻿namespace Word_Find_Solver
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadwordgrid = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.createbutton = new System.Windows.Forms.Button();
            this.createheightTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.createwidthTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.clearbutton = new System.Windows.Forms.Button();
            this.savebutton = new System.Windows.Forms.Button();
            this.foundwords = new System.Windows.Forms.ListBox();
            this.manualentry = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lastletterllRB = new System.Windows.Forms.RadioButton();
            this.crosswordRB = new System.Windows.Forms.RadioButton();
            this.sortlengthbutton = new System.Windows.Forms.Button();
            this.sortscorebutton = new System.Windows.Forms.Button();
            this.grid = new ANDREICSLIB.PanelUpdates();
            this.label4 = new System.Windows.Forms.Label();
            this.sortnamebutton = new System.Windows.Forms.Button();
            this.randomlettersbutton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Silver;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(414, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // loadwordgrid
            // 
            this.loadwordgrid.Location = new System.Drawing.Point(6, 48);
            this.loadwordgrid.Name = "loadwordgrid";
            this.loadwordgrid.Size = new System.Drawing.Size(87, 23);
            this.loadwordgrid.TabIndex = 1;
            this.loadwordgrid.Text = "Load From File";
            this.loadwordgrid.UseVisualStyleBackColor = true;
            this.loadwordgrid.Click += new System.EventHandler(this.loadwordgrid_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.randomlettersbutton);
            this.groupBox1.Controls.Add(this.createbutton);
            this.groupBox1.Controls.Add(this.createheightTB);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.createwidthTB);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.clearbutton);
            this.groupBox1.Controls.Add(this.savebutton);
            this.groupBox1.Controls.Add(this.loadwordgrid);
            this.groupBox1.Location = new System.Drawing.Point(12, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(390, 77);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Step 1: Load or create word grid";
            // 
            // createbutton
            // 
            this.createbutton.Location = new System.Drawing.Point(262, 15);
            this.createbutton.Name = "createbutton";
            this.createbutton.Size = new System.Drawing.Size(122, 23);
            this.createbutton.TabIndex = 8;
            this.createbutton.Text = "Create New Grid";
            this.createbutton.UseVisualStyleBackColor = true;
            this.createbutton.Click += new System.EventHandler(this.createbutton_Click);
            // 
            // createheightTB
            // 
            this.createheightTB.Location = new System.Drawing.Point(212, 17);
            this.createheightTB.Name = "createheightTB";
            this.createheightTB.Size = new System.Drawing.Size(44, 20);
            this.createheightTB.TabIndex = 7;
            this.createheightTB.Text = "10";
            this.createheightTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.createheightTB_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(134, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Create Height:";
            // 
            // createwidthTB
            // 
            this.createwidthTB.Location = new System.Drawing.Point(84, 17);
            this.createwidthTB.Name = "createwidthTB";
            this.createwidthTB.Size = new System.Drawing.Size(44, 20);
            this.createwidthTB.TabIndex = 5;
            this.createwidthTB.Text = "8";
            this.createwidthTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.createwidthTB_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Create Width:";
            // 
            // clearbutton
            // 
            this.clearbutton.Location = new System.Drawing.Point(185, 48);
            this.clearbutton.Name = "clearbutton";
            this.clearbutton.Size = new System.Drawing.Size(75, 23);
            this.clearbutton.TabIndex = 3;
            this.clearbutton.Text = "Clear";
            this.clearbutton.UseVisualStyleBackColor = true;
            this.clearbutton.Click += new System.EventHandler(this.clearbutton_Click);
            // 
            // savebutton
            // 
            this.savebutton.Location = new System.Drawing.Point(99, 48);
            this.savebutton.Name = "savebutton";
            this.savebutton.Size = new System.Drawing.Size(80, 23);
            this.savebutton.TabIndex = 2;
            this.savebutton.Text = "Save To File";
            this.savebutton.UseVisualStyleBackColor = true;
            this.savebutton.Click += new System.EventHandler(this.savebutton_Click);
            // 
            // foundwords
            // 
            this.foundwords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.foundwords.FormattingEnabled = true;
            this.foundwords.Location = new System.Drawing.Point(243, 273);
            this.foundwords.Name = "foundwords";
            this.foundwords.Size = new System.Drawing.Size(159, 160);
            this.foundwords.TabIndex = 5;
            this.foundwords.SelectedIndexChanged += new System.EventHandler(this.foundwords_SelectedIndexChanged);
            // 
            // manualentry
            // 
            this.manualentry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.manualentry.Location = new System.Drawing.Point(243, 460);
            this.manualentry.Name = "manualentry";
            this.manualentry.Size = new System.Drawing.Size(159, 20);
            this.manualentry.TabIndex = 6;
            this.manualentry.TextChanged += new System.EventHandler(this.manualentry_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(243, 195);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Found Words List:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lastletterllRB);
            this.groupBox2.Controls.Add(this.crosswordRB);
            this.groupBox2.Location = new System.Drawing.Point(12, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(390, 69);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Step 2:Method for finding words";
            // 
            // lastletterllRB
            // 
            this.lastletterllRB.AutoSize = true;
            this.lastletterllRB.Location = new System.Drawing.Point(128, 28);
            this.lastletterllRB.Name = "lastletterllRB";
            this.lastletterllRB.Size = new System.Drawing.Size(164, 17);
            this.lastletterllRB.TabIndex = 1;
            this.lastletterllRB.Text = "Any Direction from Last Letter";
            this.lastletterllRB.UseVisualStyleBackColor = true;
            this.lastletterllRB.CheckedChanged += new System.EventHandler(this.lastletterllRB_CheckedChanged);
            // 
            // crosswordRB
            // 
            this.crosswordRB.AutoSize = true;
            this.crosswordRB.Checked = true;
            this.crosswordRB.Location = new System.Drawing.Point(19, 28);
            this.crosswordRB.Name = "crosswordRB";
            this.crosswordRB.Size = new System.Drawing.Size(74, 17);
            this.crosswordRB.TabIndex = 0;
            this.crosswordRB.TabStop = true;
            this.crosswordRB.Text = "Crossword";
            this.crosswordRB.UseVisualStyleBackColor = true;
            this.crosswordRB.CheckedChanged += new System.EventHandler(this.crosswordRB_CheckedChanged);
            // 
            // sortlengthbutton
            // 
            this.sortlengthbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sortlengthbutton.Location = new System.Drawing.Point(246, 218);
            this.sortlengthbutton.Name = "sortlengthbutton";
            this.sortlengthbutton.Size = new System.Drawing.Size(75, 23);
            this.sortlengthbutton.TabIndex = 10;
            this.sortlengthbutton.Text = "Sort Length";
            this.sortlengthbutton.UseVisualStyleBackColor = true;
            this.sortlengthbutton.Click += new System.EventHandler(this.sortlengthbutton_Click);
            // 
            // sortscorebutton
            // 
            this.sortscorebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sortscorebutton.Location = new System.Drawing.Point(327, 218);
            this.sortscorebutton.Name = "sortscorebutton";
            this.sortscorebutton.Size = new System.Drawing.Size(75, 23);
            this.sortscorebutton.TabIndex = 11;
            this.sortscorebutton.Text = "Sort Score";
            this.sortscorebutton.UseVisualStyleBackColor = true;
            this.sortscorebutton.Click += new System.EventHandler(this.sortscorebutton_Click);
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.grid.BorderColour = System.Drawing.Color.Black;
            this.grid.BorderWidth = 0;
            this.grid.Location = new System.Drawing.Point(12, 195);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(225, 285);
            this.grid.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(243, 444);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Manual Search:";
            // 
            // sortnamebutton
            // 
            this.sortnamebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sortnamebutton.Location = new System.Drawing.Point(246, 244);
            this.sortnamebutton.Name = "sortnamebutton";
            this.sortnamebutton.Size = new System.Drawing.Size(75, 23);
            this.sortnamebutton.TabIndex = 13;
            this.sortnamebutton.Text = "Sort Name";
            this.sortnamebutton.UseVisualStyleBackColor = true;
            this.sortnamebutton.Click += new System.EventHandler(this.sortnamebutton_Click);
            // 
            // randomlettersbutton
            // 
            this.randomlettersbutton.Location = new System.Drawing.Point(262, 48);
            this.randomlettersbutton.Name = "randomlettersbutton";
            this.randomlettersbutton.Size = new System.Drawing.Size(122, 23);
            this.randomlettersbutton.TabIndex = 9;
            this.randomlettersbutton.Text = "Randomise Letters";
            this.randomlettersbutton.UseVisualStyleBackColor = true;
            this.randomlettersbutton.Click += new System.EventHandler(this.randomlettersbutton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 492);
            this.Controls.Add(this.sortnamebutton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sortscorebutton);
            this.Controls.Add(this.sortlengthbutton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.manualentry);
            this.Controls.Add(this.foundwords);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(430, 530);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button loadwordgrid;
        private System.Windows.Forms.GroupBox groupBox1;
        private ANDREICSLIB.PanelUpdates grid;
        private System.Windows.Forms.ListBox foundwords;
        private System.Windows.Forms.TextBox manualentry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton lastletterllRB;
        private System.Windows.Forms.RadioButton crosswordRB;
        private System.Windows.Forms.Button sortlengthbutton;
        private System.Windows.Forms.Button sortscorebutton;
        private System.Windows.Forms.Button savebutton;
        private System.Windows.Forms.Button clearbutton;
        private System.Windows.Forms.Button createbutton;
        private System.Windows.Forms.TextBox createheightTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox createwidthTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button sortnamebutton;
        private System.Windows.Forms.Button randomlettersbutton;
    }
}

