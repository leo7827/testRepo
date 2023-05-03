
namespace STKCTest
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tlpMain_SCCMD = new System.Windows.Forms.TableLayoutPanel();
            this.butSaveCommand = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.cbbTransferMode = new System.Windows.Forms.ComboBox();
            this.label39 = new System.Windows.Forms.Label();
            this.cbbForkNumber = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tlpMain_SCCMD.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain_SCCMD
            // 
            this.tlpMain_SCCMD.ColumnCount = 3;
            this.tlpMain_SCCMD.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpMain_SCCMD.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlpMain_SCCMD.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlpMain_SCCMD.Controls.Add(this.butSaveCommand, 2, 3);
            this.tlpMain_SCCMD.Controls.Add(this.label13, 0, 3);
            this.tlpMain_SCCMD.Controls.Add(this.txtDestination, 1, 3);
            this.tlpMain_SCCMD.Controls.Add(this.label12, 0, 2);
            this.tlpMain_SCCMD.Controls.Add(this.txtSource, 1, 2);
            this.tlpMain_SCCMD.Controls.Add(this.label49, 0, 1);
            this.tlpMain_SCCMD.Controls.Add(this.cbbTransferMode, 1, 1);
            this.tlpMain_SCCMD.Controls.Add(this.label39, 0, 0);
            this.tlpMain_SCCMD.Controls.Add(this.cbbForkNumber, 1, 0);
            this.tlpMain_SCCMD.Location = new System.Drawing.Point(8, 8);
            this.tlpMain_SCCMD.Name = "tlpMain_SCCMD";
            this.tlpMain_SCCMD.RowCount = 4;
            this.tlpMain_SCCMD.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpMain_SCCMD.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpMain_SCCMD.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpMain_SCCMD.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpMain_SCCMD.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain_SCCMD.Size = new System.Drawing.Size(424, 159);
            this.tlpMain_SCCMD.TabIndex = 32;
            // 
            // butSaveCommand
            // 
            this.butSaveCommand.Dock = System.Windows.Forms.DockStyle.Left;
            this.butSaveCommand.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butSaveCommand.Image = ((System.Drawing.Image)(resources.GetObject("butSaveCommand.Image")));
            this.butSaveCommand.Location = new System.Drawing.Point(285, 120);
            this.butSaveCommand.Name = "butSaveCommand";
            this.butSaveCommand.Size = new System.Drawing.Size(116, 36);
            this.butSaveCommand.TabIndex = 29;
            this.butSaveCommand.Text = " &Save";
            this.butSaveCommand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.butSaveCommand.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butSaveCommand.UseVisualStyleBackColor = true;
            this.butSaveCommand.Click += new System.EventHandler(this.butSaveCommand_Click);
            // 
            // label13
            // 
            this.label13.Dock = System.Windows.Forms.DockStyle.Right;
            this.label13.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.ForeColor = System.Drawing.Color.DarkRed;
            this.label13.Location = new System.Drawing.Point(3, 117);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(135, 42);
            this.label13.TabIndex = 18;
            this.label13.Text = "Destination";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDestination.Location = new System.Drawing.Point(144, 123);
            this.txtDestination.MaxLength = 7;
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(135, 29);
            this.txtDestination.TabIndex = 27;
            this.txtDestination.Text = "0";
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Right;
            this.label12.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.ForeColor = System.Drawing.Color.DarkRed;
            this.label12.Location = new System.Drawing.Point(3, 78);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(135, 39);
            this.label12.TabIndex = 19;
            this.label12.Text = "Source";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSource.Location = new System.Drawing.Point(144, 83);
            this.txtSource.MaxLength = 7;
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(135, 29);
            this.txtSource.TabIndex = 26;
            this.txtSource.Text = "0";
            // 
            // label49
            // 
            this.label49.Dock = System.Windows.Forms.DockStyle.Right;
            this.label49.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label49.ForeColor = System.Drawing.Color.DarkRed;
            this.label49.Location = new System.Drawing.Point(3, 39);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(135, 39);
            this.label49.TabIndex = 15;
            this.label49.Text = "Transfer Mode";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbbTransferMode
            // 
            this.cbbTransferMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbTransferMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbTransferMode.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cbbTransferMode.FormattingEnabled = true;
            this.cbbTransferMode.Location = new System.Drawing.Point(144, 44);
            this.cbbTransferMode.Name = "cbbTransferMode";
            this.cbbTransferMode.Size = new System.Drawing.Size(135, 28);
            this.cbbTransferMode.TabIndex = 17;
            // 
            // label39
            // 
            this.label39.Dock = System.Windows.Forms.DockStyle.Right;
            this.label39.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label39.ForeColor = System.Drawing.Color.DarkRed;
            this.label39.Location = new System.Drawing.Point(3, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(135, 39);
            this.label39.TabIndex = 22;
            this.label39.Text = "Fork Number";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbbForkNumber
            // 
            this.cbbForkNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbForkNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbForkNumber.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cbbForkNumber.FormattingEnabled = true;
            this.cbbForkNumber.Location = new System.Drawing.Point(144, 5);
            this.cbbForkNumber.Name = "cbbForkNumber";
            this.cbbForkNumber.Size = new System.Drawing.Size(135, 28);
            this.cbbForkNumber.TabIndex = 24;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 173);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(424, 403);
            this.dataGridView1.TabIndex = 33;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(438, 490);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 36);
            this.button1.TabIndex = 34;
            this.button1.Text = "Start";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(438, 532);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(116, 36);
            this.button2.TabIndex = 35;
            this.button2.Text = "Stop";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 588);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tlpMain_SCCMD);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tlpMain_SCCMD.ResumeLayout(false);
            this.tlpMain_SCCMD.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain_SCCMD;
        private System.Windows.Forms.Button butSaveCommand;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.ComboBox cbbTransferMode;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ComboBox cbbForkNumber;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

