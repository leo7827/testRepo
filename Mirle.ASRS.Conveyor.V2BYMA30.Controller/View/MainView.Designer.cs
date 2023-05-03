namespace Mirle.ASRS.Conveyor.V2BYMA30_3F.View
{
    partial class MainView
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.butMain_BufferPLCInfo = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.butMain_Layout = new System.Windows.Forms.Button();
            this.lblPLCConnSts = new System.Windows.Forms.Label();
            this.timMainProc = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1370, 729);
            this.splitContainer1.SplitterDistance = 1186;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.butMain_BufferPLCInfo, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.butMain_Layout, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblPLCConnSts, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(176, 727);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // butMain_BufferPLCInfo
            // 
            this.butMain_BufferPLCInfo.BackColor = System.Drawing.Color.Gainsboro;
            this.butMain_BufferPLCInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.butMain_BufferPLCInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butMain_BufferPLCInfo.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butMain_BufferPLCInfo.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.butMain_BufferPLCInfo.Location = new System.Drawing.Point(0, 188);
            this.butMain_BufferPLCInfo.Margin = new System.Windows.Forms.Padding(0);
            this.butMain_BufferPLCInfo.Name = "butMain_BufferPLCInfo";
            this.butMain_BufferPLCInfo.Size = new System.Drawing.Size(176, 63);
            this.butMain_BufferPLCInfo.TabIndex = 39;
            this.butMain_BufferPLCInfo.Text = "Buffer PLC Info";
            this.butMain_BufferPLCInfo.UseVisualStyleBackColor = false;
            this.butMain_BufferPLCInfo.Click += new System.EventHandler(this.butMain_BufferPLCInfo_Click);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Black;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(2, 2);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(172, 42);
            this.label9.TabIndex = 10;
            this.label9.Text = "PLC Sts";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // butMain_Layout
            // 
            this.butMain_Layout.BackColor = System.Drawing.Color.Aqua;
            this.butMain_Layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.butMain_Layout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butMain_Layout.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butMain_Layout.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.butMain_Layout.Location = new System.Drawing.Point(0, 125);
            this.butMain_Layout.Margin = new System.Windows.Forms.Padding(0);
            this.butMain_Layout.Name = "butMain_Layout";
            this.butMain_Layout.Size = new System.Drawing.Size(176, 63);
            this.butMain_Layout.TabIndex = 38;
            this.butMain_Layout.Text = "Layout";
            this.butMain_Layout.UseVisualStyleBackColor = false;
            this.butMain_Layout.Click += new System.EventHandler(this.butMain_Layout_Click);
            // 
            // lblPLCConnSts
            // 
            this.lblPLCConnSts.BackColor = System.Drawing.Color.Red;
            this.lblPLCConnSts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPLCConnSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPLCConnSts.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblPLCConnSts.ForeColor = System.Drawing.Color.Black;
            this.lblPLCConnSts.Location = new System.Drawing.Point(2, 48);
            this.lblPLCConnSts.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lblPLCConnSts.Name = "lblPLCConnSts";
            this.lblPLCConnSts.Size = new System.Drawing.Size(172, 75);
            this.lblPLCConnSts.TabIndex = 12;
            this.lblPLCConnSts.Text = "Connect Sts";
            this.lblPLCConnSts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timMainProc
            // 
            this.timMainProc.Interval = 500;
            this.timMainProc.Tick += new System.EventHandler(this.timMainProc_Tick);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 729);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainView";
            this.Text = "MainView";
            this.Load += new System.EventHandler(this.MainView_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Timer timMainProc;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblPLCConnSts;
        private System.Windows.Forms.Button butMain_Layout;
        private System.Windows.Forms.Button butMain_BufferPLCInfo;
    }
}