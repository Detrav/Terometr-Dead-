namespace SnifferGUI.Forms
{
    partial class ViewValueForm
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
            this.numericUpDownByteNumber = new System.Windows.Forms.NumericUpDown();
            this.labelForByteNumber = new System.Windows.Forms.Label();
            this.comboBoxDataType = new System.Windows.Forms.ComboBox();
            this.labelForDataType = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.labelForSize = new System.Windows.Forms.Label();
            this.numericUpDownSize = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownByteNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownByteNumber
            // 
            this.numericUpDownByteNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownByteNumber.Location = new System.Drawing.Point(12, 25);
            this.numericUpDownByteNumber.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.numericUpDownByteNumber.Name = "numericUpDownByteNumber";
            this.numericUpDownByteNumber.Size = new System.Drawing.Size(250, 20);
            this.numericUpDownByteNumber.TabIndex = 0;
            this.numericUpDownByteNumber.ValueChanged += new System.EventHandler(this.numericUpDownByteNumber_ValueChanged);
            // 
            // labelForByteNumber
            // 
            this.labelForByteNumber.AutoSize = true;
            this.labelForByteNumber.Location = new System.Drawing.Point(12, 9);
            this.labelForByteNumber.Name = "labelForByteNumber";
            this.labelForByteNumber.Size = new System.Drawing.Size(76, 13);
            this.labelForByteNumber.TabIndex = 1;
            this.labelForByteNumber.Text = "Номер байта:";
            // 
            // comboBoxDataType
            // 
            this.comboBoxDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDataType.FormattingEnabled = true;
            this.comboBoxDataType.Items.AddRange(new object[] {
            "bitarray",
            "byte",
            "sbyte",
            "ushort",
            "short",
            "uint",
            "int",
            "ulong",
            "long",
            "float",
            "double",
            "char",
            "string",
            "bool",
            "hex"});
            this.comboBoxDataType.Location = new System.Drawing.Point(12, 105);
            this.comboBoxDataType.Name = "comboBoxDataType";
            this.comboBoxDataType.Size = new System.Drawing.Size(250, 21);
            this.comboBoxDataType.TabIndex = 2;
            this.comboBoxDataType.Text = "bitarray";
            this.comboBoxDataType.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataType_SelectedIndexChanged);
            // 
            // labelForDataType
            // 
            this.labelForDataType.AutoSize = true;
            this.labelForDataType.Location = new System.Drawing.Point(12, 89);
            this.labelForDataType.Name = "labelForDataType";
            this.labelForDataType.Size = new System.Drawing.Size(66, 13);
            this.labelForDataType.TabIndex = 3;
            this.labelForDataType.Text = "Тип данных";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 132);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(250, 104);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // labelForSize
            // 
            this.labelForSize.AutoSize = true;
            this.labelForSize.Location = new System.Drawing.Point(12, 51);
            this.labelForSize.Name = "labelForSize";
            this.labelForSize.Size = new System.Drawing.Size(130, 13);
            this.labelForSize.TabIndex = 6;
            this.labelForSize.Text = "Размер, для некоторых:";
            // 
            // numericUpDownSize
            // 
            this.numericUpDownSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSize.Location = new System.Drawing.Point(12, 67);
            this.numericUpDownSize.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.numericUpDownSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSize.Name = "numericUpDownSize";
            this.numericUpDownSize.Size = new System.Drawing.Size(250, 20);
            this.numericUpDownSize.TabIndex = 5;
            this.numericUpDownSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSize.ValueChanged += new System.EventHandler(this.numericUpDownSize_ValueChanged);
            // 
            // ViewValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 248);
            this.Controls.Add(this.labelForSize);
            this.Controls.Add(this.numericUpDownSize);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.labelForDataType);
            this.Controls.Add(this.comboBoxDataType);
            this.Controls.Add(this.labelForByteNumber);
            this.Controls.Add(this.numericUpDownByteNumber);
            this.Name = "ViewValueForm";
            this.Text = "ViewValueForm";
            this.Shown += new System.EventHandler(this.ViewValueForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownByteNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownByteNumber;
        private System.Windows.Forms.Label labelForByteNumber;
        private System.Windows.Forms.ComboBox comboBoxDataType;
        private System.Windows.Forms.Label labelForDataType;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label labelForSize;
        private System.Windows.Forms.NumericUpDown numericUpDownSize;
    }
}