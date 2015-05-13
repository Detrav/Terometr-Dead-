namespace SnifferGUI.Forms
{
    partial class FiltersForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxWhiteList = new System.Windows.Forms.CheckBox();
            this.checkBoxBlackList = new System.Windows.Forms.CheckBox();
            this.panelWhite = new System.Windows.Forms.Panel();
            this.panelBlack = new System.Windows.Forms.Panel();
            this.listBoxPacketsNameForWhite = new System.Windows.Forms.ListBox();
            this.listBoxWhite = new System.Windows.Forms.ListBox();
            this.listBoxPacketsNameForBlack = new System.Windows.Forms.ListBox();
            this.listBoxBlack = new System.Windows.Forms.ListBox();
            this.buttonAddBlack = new System.Windows.Forms.Button();
            this.buttonRemoveBlack = new System.Windows.Forms.Button();
            this.buttonRemoveWhite = new System.Windows.Forms.Button();
            this.buttonAddWhite = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panelWhite.SuspendLayout();
            this.panelBlack.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 469);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(547, 425);
            this.panel2.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(466, 434);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 434);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(547, 425);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panelWhite);
            this.tabPage1.Controls.Add(this.checkBoxWhiteList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(539, 399);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "WhiteList";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panelBlack);
            this.tabPage2.Controls.Add(this.checkBoxBlackList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(539, 399);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "BlackList";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBoxWhiteList
            // 
            this.checkBoxWhiteList.AutoSize = true;
            this.checkBoxWhiteList.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxWhiteList.Location = new System.Drawing.Point(3, 3);
            this.checkBoxWhiteList.Name = "checkBoxWhiteList";
            this.checkBoxWhiteList.Size = new System.Drawing.Size(533, 30);
            this.checkBoxWhiteList.TabIndex = 0;
            this.checkBoxWhiteList.Text = "Включить WhiteList?\r\nПри этом BlackList будет отключён!";
            this.checkBoxWhiteList.UseVisualStyleBackColor = true;
            this.checkBoxWhiteList.CheckedChanged += new System.EventHandler(this.checkBoxWhiteList_CheckedChanged);
            // 
            // checkBoxBlackList
            // 
            this.checkBoxBlackList.AutoSize = true;
            this.checkBoxBlackList.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxBlackList.Location = new System.Drawing.Point(3, 3);
            this.checkBoxBlackList.Name = "checkBoxBlackList";
            this.checkBoxBlackList.Size = new System.Drawing.Size(533, 30);
            this.checkBoxBlackList.TabIndex = 0;
            this.checkBoxBlackList.Text = "Включить BlackList?\r\nПри этом WhiteList будет отключён!";
            this.checkBoxBlackList.UseVisualStyleBackColor = true;
            this.checkBoxBlackList.CheckedChanged += new System.EventHandler(this.checkBoxBlackList_CheckedChanged);
            // 
            // panelWhite
            // 
            this.panelWhite.Controls.Add(this.buttonRemoveWhite);
            this.panelWhite.Controls.Add(this.buttonAddWhite);
            this.panelWhite.Controls.Add(this.listBoxWhite);
            this.panelWhite.Controls.Add(this.listBoxPacketsNameForWhite);
            this.panelWhite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelWhite.Enabled = false;
            this.panelWhite.Location = new System.Drawing.Point(3, 33);
            this.panelWhite.Name = "panelWhite";
            this.panelWhite.Size = new System.Drawing.Size(533, 363);
            this.panelWhite.TabIndex = 1;
            // 
            // panelBlack
            // 
            this.panelBlack.Controls.Add(this.buttonRemoveBlack);
            this.panelBlack.Controls.Add(this.buttonAddBlack);
            this.panelBlack.Controls.Add(this.listBoxBlack);
            this.panelBlack.Controls.Add(this.listBoxPacketsNameForBlack);
            this.panelBlack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBlack.Enabled = false;
            this.panelBlack.Location = new System.Drawing.Point(3, 33);
            this.panelBlack.Name = "panelBlack";
            this.panelBlack.Size = new System.Drawing.Size(533, 363);
            this.panelBlack.TabIndex = 1;
            // 
            // listBoxPacketsNameForWhite
            // 
            this.listBoxPacketsNameForWhite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxPacketsNameForWhite.FormattingEnabled = true;
            this.listBoxPacketsNameForWhite.Location = new System.Drawing.Point(3, 4);
            this.listBoxPacketsNameForWhite.Name = "listBoxPacketsNameForWhite";
            this.listBoxPacketsNameForWhite.Size = new System.Drawing.Size(250, 355);
            this.listBoxPacketsNameForWhite.TabIndex = 0;
            // 
            // listBoxWhite
            // 
            this.listBoxWhite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxWhite.FormattingEnabled = true;
            this.listBoxWhite.Location = new System.Drawing.Point(280, 4);
            this.listBoxWhite.Name = "listBoxWhite";
            this.listBoxWhite.Size = new System.Drawing.Size(250, 355);
            this.listBoxWhite.TabIndex = 1;
            // 
            // listBoxPacketsNameForBlack
            // 
            this.listBoxPacketsNameForBlack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxPacketsNameForBlack.FormattingEnabled = true;
            this.listBoxPacketsNameForBlack.Location = new System.Drawing.Point(3, 4);
            this.listBoxPacketsNameForBlack.Name = "listBoxPacketsNameForBlack";
            this.listBoxPacketsNameForBlack.Size = new System.Drawing.Size(250, 355);
            this.listBoxPacketsNameForBlack.TabIndex = 0;
            // 
            // listBoxBlack
            // 
            this.listBoxBlack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxBlack.FormattingEnabled = true;
            this.listBoxBlack.Location = new System.Drawing.Point(280, 4);
            this.listBoxBlack.Name = "listBoxBlack";
            this.listBoxBlack.Size = new System.Drawing.Size(250, 355);
            this.listBoxBlack.TabIndex = 1;
            // 
            // buttonAddBlack
            // 
            this.buttonAddBlack.Location = new System.Drawing.Point(259, 6);
            this.buttonAddBlack.Name = "buttonAddBlack";
            this.buttonAddBlack.Size = new System.Drawing.Size(15, 126);
            this.buttonAddBlack.TabIndex = 2;
            this.buttonAddBlack.Text = "Добавить";
            this.buttonAddBlack.UseVisualStyleBackColor = true;
            this.buttonAddBlack.Click += new System.EventHandler(this.buttonAddBlack_Click);
            // 
            // buttonRemoveBlack
            // 
            this.buttonRemoveBlack.Location = new System.Drawing.Point(259, 233);
            this.buttonRemoveBlack.Name = "buttonRemoveBlack";
            this.buttonRemoveBlack.Size = new System.Drawing.Size(15, 126);
            this.buttonRemoveBlack.TabIndex = 3;
            this.buttonRemoveBlack.Text = "Убрать";
            this.buttonRemoveBlack.UseVisualStyleBackColor = true;
            this.buttonRemoveBlack.Click += new System.EventHandler(this.buttonRemoveBlack_Click);
            // 
            // buttonRemoveWhite
            // 
            this.buttonRemoveWhite.Location = new System.Drawing.Point(259, 232);
            this.buttonRemoveWhite.Name = "buttonRemoveWhite";
            this.buttonRemoveWhite.Size = new System.Drawing.Size(15, 126);
            this.buttonRemoveWhite.TabIndex = 5;
            this.buttonRemoveWhite.Text = "Убрать";
            this.buttonRemoveWhite.UseVisualStyleBackColor = true;
            this.buttonRemoveWhite.Click += new System.EventHandler(this.buttonRemoveWhite_Click);
            // 
            // buttonAddWhite
            // 
            this.buttonAddWhite.Location = new System.Drawing.Point(259, 5);
            this.buttonAddWhite.Name = "buttonAddWhite";
            this.buttonAddWhite.Size = new System.Drawing.Size(15, 126);
            this.buttonAddWhite.TabIndex = 4;
            this.buttonAddWhite.Text = "Добавить";
            this.buttonAddWhite.UseVisualStyleBackColor = true;
            this.buttonAddWhite.Click += new System.EventHandler(this.buttonAddWhite_Click);
            // 
            // FiltersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 469);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FiltersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FiltersForm";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panelWhite.ResumeLayout(false);
            this.panelBlack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panelWhite;
        private System.Windows.Forms.Button buttonRemoveWhite;
        private System.Windows.Forms.Button buttonAddWhite;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panelBlack;
        private System.Windows.Forms.Button buttonRemoveBlack;
        private System.Windows.Forms.Button buttonAddBlack;
        public System.Windows.Forms.ListBox listBoxWhite;
        public System.Windows.Forms.ListBox listBoxPacketsNameForWhite;
        public System.Windows.Forms.CheckBox checkBoxWhiteList;
        public System.Windows.Forms.ListBox listBoxBlack;
        public System.Windows.Forms.ListBox listBoxPacketsNameForBlack;
        public System.Windows.Forms.CheckBox checkBoxBlackList;
    }
}