namespace Mirle.MPLCViewer.View
{
    partial class frmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
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
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        [System.Obsolete]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dudInterval = new System.Windows.Forms.DomainUpDown();
            this.buttonFind = new System.Windows.Forms.Button();
            this.butShowTimeChartView = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonTogleHideList = new System.Windows.Forms.Button();
            this.butStart = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.butLoadFiles = new System.Windows.Forms.Button();
            this.MonitorPanel = new System.Windows.Forms.Panel();
            this.timerPlayer = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tlp2Main = new System.Windows.Forms.TableLayoutPanel();
            this.lstRawData = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslCurrentRowTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCurrentRow = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslTotalRow = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCache = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspbCaching = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslFilePath = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.timerRawCaching = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tlp2Main.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MonitorPanel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.913272F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.08673F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1054, 761);
            this.tableLayoutPanel1.TabIndex = 31;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1054, 44);
            this.tableLayoutPanel5.TabIndex = 31;
            // 
            // panel1
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.dudInterval);
            this.panel1.Controls.Add(this.buttonFind);
            this.panel1.Controls.Add(this.butShowTimeChartView);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.buttonTogleHideList);
            this.panel1.Controls.Add(this.butStart);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.butLoadFiles);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel5.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(1048, 38);
            this.panel1.TabIndex = 0;
            // 
            // dudInterval
            // 
            this.dudInterval.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dudInterval.Items.Add("1000");
            this.dudInterval.Items.Add("700");
            this.dudInterval.Items.Add("500");
            this.dudInterval.Items.Add("400");
            this.dudInterval.Items.Add("300");
            this.dudInterval.Items.Add("200");
            this.dudInterval.Items.Add("100");
            this.dudInterval.Items.Add("50");
            this.dudInterval.Items.Add("15");
            this.dudInterval.Location = new System.Drawing.Point(847, 3);
            this.dudInterval.Name = "dudInterval";
            this.dudInterval.ReadOnly = true;
            this.dudInterval.Size = new System.Drawing.Size(70, 29);
            this.dudInterval.TabIndex = 439;
            this.dudInterval.Text = "1000";
            this.dudInterval.Wrap = true;
            this.dudInterval.SelectedItemChanged += new System.EventHandler(this.dudInterval_SelectedItemChanged);
            // 
            // buttonFind
            // 
            this.buttonFind.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFind.Image = ((System.Drawing.Image)(resources.GetObject("buttonFind.Image")));
            this.buttonFind.Location = new System.Drawing.Point(958, -1);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(36, 39);
            this.buttonFind.TabIndex = 450;
            this.buttonFind.UseVisualStyleBackColor = false;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // butShowTimeChartView
            // 
            this.butShowTimeChartView.Image = ((System.Drawing.Image)(resources.GetObject("butShowTimeChartView.Image")));
            this.butShowTimeChartView.Location = new System.Drawing.Point(921, -1);
            this.butShowTimeChartView.Name = "butShowTimeChartView";
            this.butShowTimeChartView.Size = new System.Drawing.Size(36, 39);
            this.butShowTimeChartView.TabIndex = 449;
            this.butShowTimeChartView.UseVisualStyleBackColor = true;
            this.butShowTimeChartView.Click += new System.EventHandler(butShowTimeChartView_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox1.Location = new System.Drawing.Point(208, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(515, 33);
            this.textBox1.TabIndex = 448;
            // 
            // buttonTogleHideList
            // 
            this.buttonTogleHideList.Image = ((System.Drawing.Image)(resources.GetObject("buttonTogleHideList.Image")));
            this.buttonTogleHideList.Location = new System.Drawing.Point(1000, 2);
            this.buttonTogleHideList.Name = "buttonTogleHideList";
            this.buttonTogleHideList.Size = new System.Drawing.Size(39, 36);
            this.buttonTogleHideList.TabIndex = 444;
            this.buttonTogleHideList.Tag = "Start";
            this.buttonTogleHideList.UseVisualStyleBackColor = true;
            this.buttonTogleHideList.Click += new System.EventHandler(this.buttonTogleHideList_Click);
            // 
            // butStart
            // 
            this.butStart.Image = ((System.Drawing.Image)(resources.GetObject("butStart.Image")));
            this.butStart.Location = new System.Drawing.Point(723, 1);
            this.butStart.Name = "butStart";
            this.butStart.Size = new System.Drawing.Size(39, 36);
            this.butStart.TabIndex = 444;
            this.butStart.Tag = "Start";
            this.butStart.UseVisualStyleBackColor = true;
            this.butStart.Click += new System.EventHandler(this.butStart_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label9.Location = new System.Drawing.Point(770, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 20);
            this.label9.TabIndex = 441;
            this.label9.Text = "Interval : ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(134, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 20);
            this.label10.TabIndex = 442;
            this.label10.Text = "FilePath:";
            // 
            // butLoadFiles
            // 
            this.butLoadFiles.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butLoadFiles.Location = new System.Drawing.Point(3, 2);
            this.butLoadFiles.Name = "butLoadFiles";
            this.butLoadFiles.Size = new System.Drawing.Size(124, 33);
            this.butLoadFiles.TabIndex = 438;
            this.butLoadFiles.Text = "Load PLC Log";
            this.butLoadFiles.UseVisualStyleBackColor = true;
            this.butLoadFiles.Click += new System.EventHandler(this.butLoadFiles_Click);
            // 
            // MonitorPanel
            // 
            this.MonitorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MonitorPanel.Location = new System.Drawing.Point(3, 47);
            this.MonitorPanel.Name = "MonitorPanel";
            this.MonitorPanel.Size = new System.Drawing.Size(1048, 711);
            this.MonitorPanel.TabIndex = 32;
            // 
            // timerPlayer
            // 
            this.timerPlayer.Interval = 1000;
            this.timerPlayer.Tick += new System.EventHandler(this.timerPlayer_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "play_pause_resume.ico");
            this.imageList1.Images.SetKeyName(1, "pause.ico");
            // 
            // tlp2Main
            // 
            this.tlp2Main.ColumnCount = 2;
            this.tlp2Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp2Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlp2Main.Controls.Add(this.lstRawData, 1, 0);
            this.tlp2Main.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tlp2Main.Controls.Add(this.statusStrip1, 0, 1);
            this.tlp2Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp2Main.Location = new System.Drawing.Point(0, 0);
            this.tlp2Main.Name = "tlp2Main";
            this.tlp2Main.RowCount = 2;
            this.tlp2Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp2Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp2Main.Size = new System.Drawing.Size(1260, 787);
            this.tlp2Main.TabIndex = 69;
            // 
            // lstRawData
            // 
            this.lstRawData.AllowDrop = true;
            this.lstRawData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRawData.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lstRawData.FormattingEnabled = true;
            this.lstRawData.ItemHeight = 20;
            this.lstRawData.Location = new System.Drawing.Point(1063, 3);
            this.lstRawData.Name = "lstRawData";
            this.lstRawData.Size = new System.Drawing.Size(194, 761);
            this.lstRawData.TabIndex = 32;
            this.lstRawData.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstRawData_DragDrop);
            this.lstRawData.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstRawData_DragEnter);
            // 
            // statusStrip1
            // 
            this.tlp2Main.SetColumnSpan(this.statusStrip1, 2);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslCurrentRowTime,
            this.toolStripStatusLabel1,
            this.tsslCurrentRow,
            this.toolStripStatusLabel2,
            this.tsslTotalRow,
            this.toolStripStatusLabel3,
            this.tsslCache,
            this.tspbCaching,
            this.toolStripStatusLabel4,
            this.tsslFilePath});
            this.statusStrip1.Location = new System.Drawing.Point(0, 767);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1260, 20);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 33;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslCurrentRowTime
            // 
            this.tsslCurrentRowTime.Name = "tsslCurrentRowTime";
            this.tsslCurrentRowTime.Size = new System.Drawing.Size(160, 15);
            this.tsslCurrentRowTime.Text = "Current Time : 00:00:00.00000";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 15);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // tsslCurrentRow
            // 
            this.tsslCurrentRow.Name = "tsslCurrentRow";
            this.tsslCurrentRow.Size = new System.Drawing.Size(42, 15);
            this.tsslCurrentRow.Text = "Row: 0";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(10, 15);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // tsslTotalRow
            // 
            this.tsslTotalRow.Name = "tsslTotalRow";
            this.tsslTotalRow.Size = new System.Drawing.Size(70, 15);
            this.tsslTotalRow.Text = "Total Row: 0";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(10, 15);
            this.toolStripStatusLabel3.Text = "|";
            // 
            // tsslCache
            // 
            this.tsslCache.Name = "tsslCache";
            this.tsslCache.Size = new System.Drawing.Size(99, 15);
            this.tsslCache.Text = "RawData Cached:";
            // 
            // tspbCaching
            // 
            this.tspbCaching.Name = "tspbCaching";
            this.tspbCaching.Size = new System.Drawing.Size(100, 14);
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(10, 15);
            this.toolStripStatusLabel4.Text = "|";
            // 
            // tsslFilePath
            // 
            this.tsslFilePath.Name = "tsslFilePath";
            this.tsslFilePath.Size = new System.Drawing.Size(55, 15);
            this.tsslFilePath.Text = "FilePath: ";
            // 
            // timerRefresh
            // 
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // timerRawCaching
            // 
            this.timerRawCaching.Interval = 200;
            this.timerRawCaching.Tick += new System.EventHandler(this.TimerRawCaching_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 787);
            this.Controls.Add(this.tlp2Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "STKC Raw Data Monitor For TwinFork Stocker";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tlp2Main.ResumeLayout(false);
            this.tlp2Main.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button butStart;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DomainUpDown dudInterval;
        private System.Windows.Forms.Button butLoadFiles;
        private System.Windows.Forms.Timer timerPlayer;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button butShowTimeChartView;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Button buttonTogleHideList;
        private System.Windows.Forms.TableLayoutPanel tlp2Main;
        private System.Windows.Forms.ListBox lstRawData;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Panel MonitorPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslCurrentRowTime;
        private System.Windows.Forms.ToolStripStatusLabel tsslCurrentRow;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel tsslTotalRow;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel tsslFilePath;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel tsslCache;
        private System.Windows.Forms.ToolStripProgressBar tspbCaching;
        private System.Windows.Forms.Timer timerRawCaching;
    }
}

