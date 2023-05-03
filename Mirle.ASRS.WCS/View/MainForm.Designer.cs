namespace Mirle.ASRS.WCS.View
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tlpMainSts = new System.Windows.Forms.TableLayoutPanel();
            this.lblTimer = new System.Windows.Forms.Label();
            this.picMirle = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDBConn_WMS = new System.Windows.Forms.Label();
            this.chkOnline = new System.Windows.Forms.CheckBox();
            this.lblDBConn_WCS = new System.Windows.Forms.Label();
            this.spcView = new System.Windows.Forms.SplitContainer();
            this.spcMainView = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkIgnoreTkt = new System.Windows.Forms.CheckBox();
            this.btnCmdMaintain = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCall8FCV = new System.Windows.Forms.Button();
            this.btnCall10FCV = new System.Windows.Forms.Button();
            this.btnCallEleCV = new System.Windows.Forms.Button();
            this.chkOPC = new System.Windows.Forms.CheckBox();
            this.Grid1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tlpMainSts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMirle)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcView)).BeginInit();
            this.spcView.Panel1.SuspendLayout();
            this.spcView.Panel2.SuspendLayout();
            this.spcView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMainView)).BeginInit();
            this.spcMainView.Panel2.SuspendLayout();
            this.spcMainView.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tlpMainSts);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.spcView);
            this.splitContainer1.Size = new System.Drawing.Size(1171, 598);
            this.splitContainer1.SplitterDistance = 79;
            this.splitContainer1.TabIndex = 0;
            // 
            // tlpMainSts
            // 
            this.tlpMainSts.ColumnCount = 4;
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.tlpMainSts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tlpMainSts.Controls.Add(this.lblTimer, 0, 0);
            this.tlpMainSts.Controls.Add(this.picMirle, 0, 0);
            this.tlpMainSts.Controls.Add(this.tableLayoutPanel2, 3, 0);
            this.tlpMainSts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainSts.Location = new System.Drawing.Point(0, 0);
            this.tlpMainSts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlpMainSts.Name = "tlpMainSts";
            this.tlpMainSts.RowCount = 1;
            this.tlpMainSts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainSts.Size = new System.Drawing.Size(1171, 79);
            this.tlpMainSts.TabIndex = 0;
            // 
            // lblTimer
            // 
            this.lblTimer.BackColor = System.Drawing.SystemColors.Control;
            this.lblTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTimer.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.ForeColor = System.Drawing.Color.Black;
            this.lblTimer.Location = new System.Drawing.Point(166, 0);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(157, 79);
            this.lblTimer.TabIndex = 268;
            this.lblTimer.Text = "yyyy/MM/dd hh:mm:ss";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picMirle
            // 
            this.picMirle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picMirle.Image = ((System.Drawing.Image)(resources.GetObject("picMirle.Image")));
            this.picMirle.Location = new System.Drawing.Point(3, 2);
            this.picMirle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picMirle.Name = "picMirle";
            this.picMirle.Size = new System.Drawing.Size(157, 75);
            this.picMirle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMirle.TabIndex = 267;
            this.picMirle.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.lblDBConn_WMS, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkOnline, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblDBConn_WCS, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1008, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(160, 75);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblDBConn_WMS
            // 
            this.lblDBConn_WMS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDBConn_WMS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDBConn_WMS.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDBConn_WMS.Location = new System.Drawing.Point(3, 24);
            this.lblDBConn_WMS.Name = "lblDBConn_WMS";
            this.lblDBConn_WMS.Size = new System.Drawing.Size(154, 24);
            this.lblDBConn_WMS.TabIndex = 3;
            this.lblDBConn_WMS.Text = "WMS DB Sts";
            this.lblDBConn_WMS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDBConn_WMS.Visible = false;
            // 
            // chkOnline
            // 
            this.chkOnline.AutoSize = true;
            this.chkOnline.Checked = true;
            this.chkOnline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkOnline.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkOnline.Location = new System.Drawing.Point(3, 50);
            this.chkOnline.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkOnline.Name = "chkOnline";
            this.chkOnline.Size = new System.Drawing.Size(154, 23);
            this.chkOnline.TabIndex = 2;
            this.chkOnline.Text = "OnLine";
            this.chkOnline.UseVisualStyleBackColor = true;
            this.chkOnline.Visible = false;
            this.chkOnline.CheckedChanged += new System.EventHandler(this.chkOnline_CheckedChanged);
            // 
            // lblDBConn_WCS
            // 
            this.lblDBConn_WCS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDBConn_WCS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDBConn_WCS.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDBConn_WCS.Location = new System.Drawing.Point(3, 0);
            this.lblDBConn_WCS.Name = "lblDBConn_WCS";
            this.lblDBConn_WCS.Size = new System.Drawing.Size(154, 24);
            this.lblDBConn_WCS.TabIndex = 1;
            this.lblDBConn_WCS.Text = "LOCAL DB Sts";
            this.lblDBConn_WCS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // spcView
            // 
            this.spcView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcView.Location = new System.Drawing.Point(0, 0);
            this.spcView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spcView.Name = "spcView";
            this.spcView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcView.Panel1
            // 
            this.spcView.Panel1.Controls.Add(this.spcMainView);
            // 
            // spcView.Panel2
            // 
            this.spcView.Panel2.Controls.Add(this.Grid1);
            this.spcView.Size = new System.Drawing.Size(1171, 515);
            this.spcView.SplitterDistance = 377;
            this.spcView.TabIndex = 0;
            // 
            // spcMainView
            // 
            this.spcMainView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spcMainView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMainView.Location = new System.Drawing.Point(0, 0);
            this.spcMainView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spcMainView.Name = "spcMainView";
            // 
            // spcMainView.Panel2
            // 
            this.spcMainView.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.spcMainView.Size = new System.Drawing.Size(1171, 377);
            this.spcMainView.SplitterDistance = 1047;
            this.spcMainView.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chkIgnoreTkt, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.btnCmdMaintain, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCall8FCV, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCall10FCV, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnCallEleCV, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.chkOPC, 0, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(118, 375);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // chkIgnoreTkt
            // 
            this.chkIgnoreTkt.AutoSize = true;
            this.chkIgnoreTkt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkIgnoreTkt.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkIgnoreTkt.Location = new System.Drawing.Point(3, 268);
            this.chkIgnoreTkt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkIgnoreTkt.Name = "chkIgnoreTkt";
            this.chkIgnoreTkt.Size = new System.Drawing.Size(112, 26);
            this.chkIgnoreTkt.TabIndex = 10;
            this.chkIgnoreTkt.Text = "Ignore Ticket";
            this.chkIgnoreTkt.UseVisualStyleBackColor = true;
            this.chkIgnoreTkt.Visible = false;
            // 
            // btnCmdMaintain
            // 
            this.btnCmdMaintain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCmdMaintain.Location = new System.Drawing.Point(3, 28);
            this.btnCmdMaintain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCmdMaintain.Name = "btnCmdMaintain";
            this.btnCmdMaintain.Size = new System.Drawing.Size(112, 38);
            this.btnCmdMaintain.TabIndex = 4;
            this.btnCmdMaintain.Text = "Command Maintain";
            this.btnCmdMaintain.UseVisualStyleBackColor = true;
            this.btnCmdMaintain.Click += new System.EventHandler(this.btnCmdMaintain_Click);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 2);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 22);
            this.button1.TabIndex = 3;
            this.button1.Text = "Send API Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCall8FCV
            // 
            this.btnCall8FCV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCall8FCV.Location = new System.Drawing.Point(3, 70);
            this.btnCall8FCV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCall8FCV.Name = "btnCall8FCV";
            this.btnCall8FCV.Size = new System.Drawing.Size(112, 38);
            this.btnCall8FCV.TabIndex = 0;
            this.btnCall8FCV.Text = "8F CV";
            this.btnCall8FCV.UseVisualStyleBackColor = true;
            this.btnCall8FCV.Click += new System.EventHandler(this.btnCall8FCV_Click);
            // 
            // btnCall10FCV
            // 
            this.btnCall10FCV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCall10FCV.Location = new System.Drawing.Point(3, 112);
            this.btnCall10FCV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCall10FCV.Name = "btnCall10FCV";
            this.btnCall10FCV.Size = new System.Drawing.Size(112, 38);
            this.btnCall10FCV.TabIndex = 5;
            this.btnCall10FCV.Text = "10F CV";
            this.btnCall10FCV.UseVisualStyleBackColor = true;
            this.btnCall10FCV.Click += new System.EventHandler(this.btnCall10FCV_Click);
            // 
            // btnCallEleCV
            // 
            this.btnCallEleCV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCallEleCV.Location = new System.Drawing.Point(2, 154);
            this.btnCallEleCV.Margin = new System.Windows.Forms.Padding(2);
            this.btnCallEleCV.Name = "btnCallEleCV";
            this.btnCallEleCV.Size = new System.Drawing.Size(114, 38);
            this.btnCallEleCV.TabIndex = 8;
            this.btnCallEleCV.Text = "Ele CV";
            this.btnCallEleCV.UseVisualStyleBackColor = true;
            this.btnCallEleCV.Click += new System.EventHandler(this.btnCallEleCV_Click);
            // 
            // chkOPC
            // 
            this.chkOPC.AutoSize = true;
            this.chkOPC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkOPC.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkOPC.Location = new System.Drawing.Point(3, 232);
            this.chkOPC.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkOPC.Name = "chkOPC";
            this.chkOPC.Size = new System.Drawing.Size(112, 32);
            this.chkOPC.TabIndex = 7;
            this.chkOPC.Text = "OPC";
            this.chkOPC.UseVisualStyleBackColor = true;
            this.chkOPC.Visible = false;
            this.chkOPC.CheckedChanged += new System.EventHandler(this.chkOPC_CheckedChanged);
            // 
            // Grid1
            // 
            this.Grid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid1.Location = new System.Drawing.Point(0, 0);
            this.Grid1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Grid1.Name = "Grid1";
            this.Grid1.RowHeadersWidth = 62;
            this.Grid1.RowTemplate.Height = 24;
            this.Grid1.Size = new System.Drawing.Size(1171, 134);
            this.Grid1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1171, 598);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "LIFT4C";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tlpMainSts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMirle)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.spcView.Panel1.ResumeLayout(false);
            this.spcView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcView)).EndInit();
            this.spcView.ResumeLayout(false);
            this.spcMainView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMainView)).EndInit();
            this.spcMainView.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Grid1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblDBConn_WCS;
        private System.Windows.Forms.SplitContainer spcView;
        private System.Windows.Forms.DataGridView Grid1;
        private System.Windows.Forms.CheckBox chkOnline;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer spcMainView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCall8FCV;
        private System.Windows.Forms.TableLayoutPanel tlpMainSts;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox picMirle;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnCmdMaintain;
        private System.Windows.Forms.Button btnCall10FCV;
        private System.Windows.Forms.CheckBox chkOPC;
        private System.Windows.Forms.Button btnCallEleCV;
        private System.Windows.Forms.CheckBox chkIgnoreTkt;
        private System.Windows.Forms.Label lblDBConn_WMS;
    }
}

