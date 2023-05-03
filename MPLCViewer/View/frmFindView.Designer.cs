namespace Mirle.MPLCViewer.View
{
    partial class frmFindView
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
            this.buttonFind = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonBit = new System.Windows.Forms.RadioButton();
            this.radioButtonWord = new System.Windows.Forms.RadioButton();
            this.radioButtonDWord = new System.Windows.Forms.RadioButton();
            this.radioButtonWordBlock = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLength = new System.Windows.Forms.TextBox();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.textBoxEqualsValue = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBoxOption = new System.Windows.Forms.GroupBox();
            this.radioButtonHexadecimal = new System.Windows.Forms.RadioButton();
            this.radioButtonDecimal = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonDifferent = new System.Windows.Forms.RadioButton();
            this.radioButtonNotEquals = new System.Windows.Forms.RadioButton();
            this.radioButtonEquals = new System.Windows.Forms.RadioButton();
            this.buttonExportVibration = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBoxOption.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFind
            // 
            this.buttonFind.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.buttonFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFind.Location = new System.Drawing.Point(321, 42);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(92, 40);
            this.buttonFind.TabIndex = 0;
            this.buttonFind.Text = "Find";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.ButtonFind_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 179);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(399, 401);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellClick);
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAddress.Location = new System.Drawing.Point(90, 9);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(100, 26);
            this.textBoxAddress.TabIndex = 2;
            this.textBoxAddress.Text = "D5000.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Address:";
            // 
            // radioButtonBit
            // 
            this.radioButtonBit.AutoSize = true;
            this.radioButtonBit.Checked = true;
            this.radioButtonBit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonBit.Location = new System.Drawing.Point(12, 18);
            this.radioButtonBit.Name = "radioButtonBit";
            this.radioButtonBit.Size = new System.Drawing.Size(41, 20);
            this.radioButtonBit.TabIndex = 4;
            this.radioButtonBit.TabStop = true;
            this.radioButtonBit.Text = "Bit";
            this.radioButtonBit.UseVisualStyleBackColor = true;
            // 
            // radioButtonWord
            // 
            this.radioButtonWord.AutoSize = true;
            this.radioButtonWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonWord.Location = new System.Drawing.Point(59, 18);
            this.radioButtonWord.Name = "radioButtonWord";
            this.radioButtonWord.Size = new System.Drawing.Size(59, 20);
            this.radioButtonWord.TabIndex = 4;
            this.radioButtonWord.Text = "Word";
            this.radioButtonWord.UseVisualStyleBackColor = true;
            // 
            // radioButtonDWord
            // 
            this.radioButtonDWord.AutoSize = true;
            this.radioButtonDWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonDWord.Location = new System.Drawing.Point(124, 18);
            this.radioButtonDWord.Name = "radioButtonDWord";
            this.radioButtonDWord.Size = new System.Drawing.Size(69, 20);
            this.radioButtonDWord.TabIndex = 4;
            this.radioButtonDWord.Text = "DWord";
            this.radioButtonDWord.UseVisualStyleBackColor = true;
            // 
            // radioButtonWordBlock
            // 
            this.radioButtonWordBlock.AutoSize = true;
            this.radioButtonWordBlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonWordBlock.Location = new System.Drawing.Point(199, 18);
            this.radioButtonWordBlock.Name = "radioButtonWordBlock";
            this.radioButtonWordBlock.Size = new System.Drawing.Size(93, 20);
            this.radioButtonWordBlock.TabIndex = 4;
            this.radioButtonWordBlock.Text = "WordBlock";
            this.radioButtonWordBlock.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(211, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Length:";
            // 
            // textBoxLength
            // 
            this.textBoxLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLength.Location = new System.Drawing.Point(280, 12);
            this.textBoxLength.Name = "textBoxLength";
            this.textBoxLength.Size = new System.Drawing.Size(133, 26);
            this.textBoxLength.TabIndex = 2;
            this.textBoxLength.Text = "7";
            // 
            // timerUI
            // 
            this.timerUI.Enabled = true;
            this.timerUI.Tick += new System.EventHandler(this.TimerUI_Tick);
            // 
            // textBoxEqualsValue
            // 
            this.textBoxEqualsValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEqualsValue.Location = new System.Drawing.Point(165, 146);
            this.textBoxEqualsValue.Name = "textBoxEqualsValue";
            this.textBoxEqualsValue.Size = new System.Drawing.Size(246, 26);
            this.textBoxEqualsValue.TabIndex = 2;
            this.textBoxEqualsValue.Text = "0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonWordBlock);
            this.groupBox1.Controls.Add(this.radioButtonWord);
            this.groupBox1.Controls.Add(this.radioButtonBit);
            this.groupBox1.Controls.Add(this.radioButtonDWord);
            this.groupBox1.Location = new System.Drawing.Point(16, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 42);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type";
            // 
            // groupBoxOption
            // 
            this.groupBoxOption.Controls.Add(this.radioButtonHexadecimal);
            this.groupBoxOption.Controls.Add(this.radioButtonDecimal);
            this.groupBoxOption.Location = new System.Drawing.Point(16, 87);
            this.groupBoxOption.Name = "groupBoxOption";
            this.groupBoxOption.Size = new System.Drawing.Size(299, 42);
            this.groupBoxOption.TabIndex = 6;
            this.groupBoxOption.TabStop = false;
            this.groupBoxOption.Text = "Option";
            // 
            // radioButtonHexadecimal
            // 
            this.radioButtonHexadecimal.AutoSize = true;
            this.radioButtonHexadecimal.Checked = true;
            this.radioButtonHexadecimal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonHexadecimal.Location = new System.Drawing.Point(94, 18);
            this.radioButtonHexadecimal.Name = "radioButtonHexadecimal";
            this.radioButtonHexadecimal.Size = new System.Drawing.Size(106, 20);
            this.radioButtonHexadecimal.TabIndex = 4;
            this.radioButtonHexadecimal.TabStop = true;
            this.radioButtonHexadecimal.Text = "Hexadecimal";
            this.radioButtonHexadecimal.UseVisualStyleBackColor = true;
            // 
            // radioButtonDecimal
            // 
            this.radioButtonDecimal.AutoSize = true;
            this.radioButtonDecimal.Checked = true;
            this.radioButtonDecimal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonDecimal.Location = new System.Drawing.Point(12, 18);
            this.radioButtonDecimal.Name = "radioButtonDecimal";
            this.radioButtonDecimal.Size = new System.Drawing.Size(76, 20);
            this.radioButtonDecimal.TabIndex = 4;
            this.radioButtonDecimal.TabStop = true;
            this.radioButtonDecimal.Text = "Decimal";
            this.radioButtonDecimal.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonDifferent);
            this.groupBox2.Controls.Add(this.radioButtonNotEquals);
            this.groupBox2.Controls.Add(this.radioButtonEquals);
            this.groupBox2.Location = new System.Drawing.Point(16, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(143, 40);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Equals";
            // 
            // radioButtonDifferent
            // 
            this.radioButtonDifferent.AutoSize = true;
            this.radioButtonDifferent.Checked = true;
            this.radioButtonDifferent.Location = new System.Drawing.Point(12, 18);
            this.radioButtonDifferent.Name = "radioButtonDifferent";
            this.radioButtonDifferent.Size = new System.Drawing.Size(40, 16);
            this.radioButtonDifferent.TabIndex = 1;
            this.radioButtonDifferent.TabStop = true;
            this.radioButtonDifferent.Text = "diff";
            this.radioButtonDifferent.UseVisualStyleBackColor = true;
            // 
            // radioButtonNotEquals
            // 
            this.radioButtonNotEquals.AutoSize = true;
            this.radioButtonNotEquals.Location = new System.Drawing.Point(100, 18);
            this.radioButtonNotEquals.Name = "radioButtonNotEquals";
            this.radioButtonNotEquals.Size = new System.Drawing.Size(33, 16);
            this.radioButtonNotEquals.TabIndex = 0;
            this.radioButtonNotEquals.Text = "!=";
            this.radioButtonNotEquals.UseVisualStyleBackColor = true;
            // 
            // radioButtonEquals
            // 
            this.radioButtonEquals.AutoSize = true;
            this.radioButtonEquals.Location = new System.Drawing.Point(57, 18);
            this.radioButtonEquals.Name = "radioButtonEquals";
            this.radioButtonEquals.Size = new System.Drawing.Size(35, 16);
            this.radioButtonEquals.TabIndex = 0;
            this.radioButtonEquals.Text = "==";
            this.radioButtonEquals.UseVisualStyleBackColor = true;
            // 
            // buttonExportVibration
            // 
            this.buttonExportVibration.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.buttonExportVibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.buttonExportVibration.Location = new System.Drawing.Point(321, 82);
            this.buttonExportVibration.Name = "buttonExportVibration";
            this.buttonExportVibration.Size = new System.Drawing.Size(92, 63);
            this.buttonExportVibration.TabIndex = 8;
            this.buttonExportVibration.Text = "Export Vibration Value";
            this.buttonExportVibration.UseVisualStyleBackColor = true;
            this.buttonExportVibration.Click += new System.EventHandler(this.buttonExportVibration_Click);
            // 
            // frmFindView
            // 
            this.AcceptButton = this.buttonFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 591);
            this.Controls.Add(this.buttonExportVibration);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBoxOption);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxLength);
            this.Controls.Add(this.textBoxEqualsValue);
            this.Controls.Add(this.textBoxAddress);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonFind);
            this.Name = "frmFindView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FindView";
            this.Load += new System.EventHandler(this.FrmFindView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxOption.ResumeLayout(false);
            this.groupBoxOption.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonBit;
        private System.Windows.Forms.RadioButton radioButtonWord;
        private System.Windows.Forms.RadioButton radioButtonDWord;
        private System.Windows.Forms.RadioButton radioButtonWordBlock;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLength;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.TextBox textBoxEqualsValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxOption;
        private System.Windows.Forms.RadioButton radioButtonHexadecimal;
        private System.Windows.Forms.RadioButton radioButtonDecimal;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonNotEquals;
        private System.Windows.Forms.RadioButton radioButtonEquals;
        private System.Windows.Forms.RadioButton radioButtonDifferent;
        private System.Windows.Forms.Button buttonExportVibration;
    }
}