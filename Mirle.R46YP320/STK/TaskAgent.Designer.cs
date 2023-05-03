namespace Mirle.R46YP320.STK
{
    partial class TaskAgent
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskAgent));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSECSPort = new System.Windows.Forms.Label();
            this.lblSCState = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblSECSIP = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblControlMode = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblCommunicationState = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tlpStatus = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblNowDateTime = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblSTKID = new System.Windows.Forms.Label();
            this.butExitProgram = new System.Windows.Forms.Button();
            this.tbpTransfer = new System.Windows.Forms.TabControl();
            this.tbpSysTrace = new System.Windows.Forms.TabPage();
            this.lsbTaskTrace = new System.Windows.Forms.ListBox();
            this.tbpSECSTrace = new System.Windows.Forms.TabPage();
            this.lsbSECSTrace = new System.Windows.Forms.ListBox();
            this.tbpTool = new System.Windows.Forms.TabPage();
            this.tlpTool = new System.Windows.Forms.TableLayoutPanel();
            this.butRestartSECSDriver = new System.Windows.Forms.Button();
            this.butSenS1F1_Test = new System.Windows.Forms.Button();
            this.butEnableCommunication_Test = new System.Windows.Forms.Button();
            this.butOnLineRemote_Test = new System.Windows.Forms.Button();
            this.butAuto = new System.Windows.Forms.Button();
            this.butPause = new System.Windows.Forms.Button();
            this.butOffLine_Test = new System.Windows.Forms.Button();
            this.butOnLineLocal_Test = new System.Windows.Forms.Button();
            this.butSTKC = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.timerUIRefresh = new System.Windows.Forms.Timer(this.components);
            this.nicTaskAgent = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsShowIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiShow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tabReason = new System.Windows.Forms.TabControl();
            this.tbpTaskk = new System.Windows.Forms.TabPage();
            this.lsbTaskReason = new System.Windows.Forms.ListBox();
            this.tbpUpdate = new System.Windows.Forms.TabPage();
            this.lsbUpdateReason = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tbpTransfer.SuspendLayout();
            this.tbpSysTrace.SuspendLayout();
            this.tbpSECSTrace.SuspendLayout();
            this.tbpTool.SuspendLayout();
            this.tlpTool.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.cmsShowIcon.SuspendLayout();
            this.tabReason.SuspendLayout();
            this.tbpTaskk.SuspendLayout();
            this.tbpUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.tlpStatus);
            this.splitContainer1.Panel1MinSize = 70;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.butExitProgram);
            this.splitContainer1.Panel2.Controls.Add(this.tbpTransfer);
            this.splitContainer1.Size = new System.Drawing.Size(984, 361);
            this.splitContainer1.SplitterDistance = 70;
            this.splitContainer1.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblSECSPort, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblSCState, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblSECSIP, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label8, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label14, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblControlMode, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label13, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCommunicationState, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label12, 4, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(350, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(634, 70);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DarkGreen;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(1, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 19);
            this.label2.TabIndex = 15;
            this.label2.Text = "Host IP";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSECSPort
            // 
            this.lblSECSPort.BackColor = System.Drawing.Color.Blue;
            this.lblSECSPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSECSPort.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSECSPort.ForeColor = System.Drawing.Color.White;
            this.lblSECSPort.Location = new System.Drawing.Point(143, 43);
            this.lblSECSPort.Margin = new System.Windows.Forms.Padding(1);
            this.lblSECSPort.Name = "lblSECSPort";
            this.lblSECSPort.Size = new System.Drawing.Size(124, 26);
            this.lblSECSPort.TabIndex = 17;
            this.lblSECSPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSCState
            // 
            this.lblSCState.BackColor = System.Drawing.Color.Red;
            this.lblSCState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSCState.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSCState.ForeColor = System.Drawing.Color.Black;
            this.lblSCState.Location = new System.Drawing.Point(537, 43);
            this.lblSCState.Margin = new System.Windows.Forms.Padding(1);
            this.lblSCState.Name = "lblSCState";
            this.lblSCState.Size = new System.Drawing.Size(96, 26);
            this.lblSCState.TabIndex = 23;
            this.lblSCState.Text = "Paused";
            this.lblSCState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.DarkGreen;
            this.tableLayoutPanel1.SetColumnSpan(this.label11, 5);
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.ForeColor = System.Drawing.Color.Yellow;
            this.label11.Location = new System.Drawing.Point(1, 1);
            this.label11.Margin = new System.Windows.Forms.Padding(1);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(632, 19);
            this.label11.TabIndex = 15;
            this.label11.Text = "SECS Config";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSECSIP
            // 
            this.lblSECSIP.BackColor = System.Drawing.Color.Blue;
            this.lblSECSIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSECSIP.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSECSIP.ForeColor = System.Drawing.Color.White;
            this.lblSECSIP.Location = new System.Drawing.Point(1, 43);
            this.lblSECSIP.Margin = new System.Windows.Forms.Padding(1);
            this.lblSECSIP.Name = "lblSECSIP";
            this.lblSECSIP.Size = new System.Drawing.Size(140, 26);
            this.lblSECSIP.TabIndex = 16;
            this.lblSECSIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.DarkGreen;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(143, 22);
            this.label8.Margin = new System.Windows.Forms.Padding(1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 19);
            this.label8.TabIndex = 18;
            this.label8.Text = "Port No";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.DarkGreen;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(395, 22);
            this.label14.Margin = new System.Windows.Forms.Padding(1);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(140, 19);
            this.label14.TabIndex = 26;
            this.label14.Text = "Control Mode";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblControlMode
            // 
            this.lblControlMode.BackColor = System.Drawing.Color.Red;
            this.lblControlMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblControlMode.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblControlMode.ForeColor = System.Drawing.Color.Black;
            this.lblControlMode.Location = new System.Drawing.Point(395, 43);
            this.lblControlMode.Margin = new System.Windows.Forms.Padding(1);
            this.lblControlMode.Name = "lblControlMode";
            this.lblControlMode.Size = new System.Drawing.Size(140, 26);
            this.lblControlMode.TabIndex = 22;
            this.lblControlMode.Text = "Attempt On-Line";
            this.lblControlMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.DarkGreen;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(269, 22);
            this.label13.Margin = new System.Windows.Forms.Padding(1);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(124, 19);
            this.label13.TabIndex = 25;
            this.label13.Text = "Communication";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCommunicationState
            // 
            this.lblCommunicationState.BackColor = System.Drawing.Color.Red;
            this.lblCommunicationState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCommunicationState.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCommunicationState.ForeColor = System.Drawing.Color.Black;
            this.lblCommunicationState.Location = new System.Drawing.Point(269, 43);
            this.lblCommunicationState.Margin = new System.Windows.Forms.Padding(1);
            this.lblCommunicationState.Name = "lblCommunicationState";
            this.lblCommunicationState.Size = new System.Drawing.Size(124, 26);
            this.lblCommunicationState.TabIndex = 21;
            this.lblCommunicationState.Text = "Disable";
            this.lblCommunicationState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.DarkGreen;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(537, 22);
            this.label12.Margin = new System.Windows.Forms.Padding(1);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 19);
            this.label12.TabIndex = 24;
            this.label12.Text = "SC Stsate";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tlpStatus
            // 
            this.tlpStatus.AutoSize = true;
            this.tlpStatus.ColumnCount = 3;
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpStatus.Controls.Add(this.pictureBox1, 0, 0);
            this.tlpStatus.Controls.Add(this.lblNowDateTime, 0, 1);
            this.tlpStatus.Controls.Add(this.label6, 1, 0);
            this.tlpStatus.Controls.Add(this.lblSTKID, 1, 1);
            this.tlpStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.tlpStatus.Location = new System.Drawing.Point(0, 0);
            this.tlpStatus.Margin = new System.Windows.Forms.Padding(0);
            this.tlpStatus.Name = "tlpStatus";
            this.tlpStatus.RowCount = 3;
            this.tlpStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpStatus.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpStatus.Size = new System.Drawing.Size(350, 70);
            this.tlpStatus.TabIndex = 7;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Mirle.R46YP320.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // lblNowDateTime
            // 
            this.lblNowDateTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNowDateTime.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblNowDateTime.ForeColor = System.Drawing.Color.DarkMagenta;
            this.lblNowDateTime.Location = new System.Drawing.Point(1, 41);
            this.lblNowDateTime.Margin = new System.Windows.Forms.Padding(1);
            this.lblNowDateTime.Name = "lblNowDateTime";
            this.lblNowDateTime.Size = new System.Drawing.Size(198, 28);
            this.lblNowDateTime.TabIndex = 0;
            this.lblNowDateTime.Text = "2019-03-06 23:36:37.570";
            this.lblNowDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Black;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(201, 1);
            this.label6.Margin = new System.Windows.Forms.Padding(1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(148, 38);
            this.label6.TabIndex = 1;
            this.label6.Text = "Stocker ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSTKID
            // 
            this.lblSTKID.BackColor = System.Drawing.Color.Blue;
            this.lblSTKID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSTKID.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSTKID.ForeColor = System.Drawing.Color.White;
            this.lblSTKID.Location = new System.Drawing.Point(201, 41);
            this.lblSTKID.Margin = new System.Windows.Forms.Padding(1);
            this.lblSTKID.Name = "lblSTKID";
            this.lblSTKID.Size = new System.Drawing.Size(148, 28);
            this.lblSTKID.TabIndex = 10;
            this.lblSTKID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // butExitProgram
            // 
            this.butExitProgram.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butExitProgram.Image = ((System.Drawing.Image)(resources.GetObject("butExitProgram.Image")));
            this.butExitProgram.Location = new System.Drawing.Point(885, 240);
            this.butExitProgram.Margin = new System.Windows.Forms.Padding(0);
            this.butExitProgram.Name = "butExitProgram";
            this.butExitProgram.Size = new System.Drawing.Size(90, 38);
            this.butExitProgram.TabIndex = 24;
            this.butExitProgram.Text = " &Exit";
            this.butExitProgram.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.butExitProgram.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.butExitProgram.UseVisualStyleBackColor = true;
            this.butExitProgram.Click += new System.EventHandler(this.butExitProgram_Click);
            // 
            // tbpTransfer
            // 
            this.tbpTransfer.Controls.Add(this.tbpSysTrace);
            this.tbpTransfer.Controls.Add(this.tbpSECSTrace);
            this.tbpTransfer.Controls.Add(this.tbpTool);
            this.tbpTransfer.Controls.Add(this.tabPage1);
            this.tbpTransfer.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tbpTransfer.Location = new System.Drawing.Point(0, 0);
            this.tbpTransfer.Margin = new System.Windows.Forms.Padding(0);
            this.tbpTransfer.Name = "tbpTransfer";
            this.tbpTransfer.Padding = new System.Drawing.Point(0, 0);
            this.tbpTransfer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbpTransfer.SelectedIndex = 0;
            this.tbpTransfer.Size = new System.Drawing.Size(880, 287);
            this.tbpTransfer.TabIndex = 11;
            // 
            // tbpSysTrace
            // 
            this.tbpSysTrace.Controls.Add(this.lsbTaskTrace);
            this.tbpSysTrace.Location = new System.Drawing.Point(4, 26);
            this.tbpSysTrace.Name = "tbpSysTrace";
            this.tbpSysTrace.Size = new System.Drawing.Size(872, 257);
            this.tbpSysTrace.TabIndex = 4;
            this.tbpSysTrace.Text = "Sys Trace";
            this.tbpSysTrace.UseVisualStyleBackColor = true;
            // 
            // lsbTaskTrace
            // 
            this.lsbTaskTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbTaskTrace.FormattingEnabled = true;
            this.lsbTaskTrace.HorizontalScrollbar = true;
            this.lsbTaskTrace.ItemHeight = 17;
            this.lsbTaskTrace.Location = new System.Drawing.Point(0, 0);
            this.lsbTaskTrace.Name = "lsbTaskTrace";
            this.lsbTaskTrace.Size = new System.Drawing.Size(872, 257);
            this.lsbTaskTrace.TabIndex = 0;
            this.lsbTaskTrace.MouseEnter += new System.EventHandler(this.ListBoxMouseEnter);
            this.lsbTaskTrace.MouseLeave += new System.EventHandler(this.ListBoxMouseLeave);
            // 
            // tbpSECSTrace
            // 
            this.tbpSECSTrace.Controls.Add(this.lsbSECSTrace);
            this.tbpSECSTrace.Location = new System.Drawing.Point(4, 26);
            this.tbpSECSTrace.Margin = new System.Windows.Forms.Padding(0);
            this.tbpSECSTrace.Name = "tbpSECSTrace";
            this.tbpSECSTrace.Size = new System.Drawing.Size(872, 257);
            this.tbpSECSTrace.TabIndex = 2;
            this.tbpSECSTrace.Text = "SECS Trace";
            this.tbpSECSTrace.UseVisualStyleBackColor = true;
            // 
            // lsbSECSTrace
            // 
            this.lsbSECSTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbSECSTrace.FormattingEnabled = true;
            this.lsbSECSTrace.HorizontalScrollbar = true;
            this.lsbSECSTrace.ItemHeight = 17;
            this.lsbSECSTrace.Location = new System.Drawing.Point(0, 0);
            this.lsbSECSTrace.Name = "lsbSECSTrace";
            this.lsbSECSTrace.Size = new System.Drawing.Size(872, 257);
            this.lsbSECSTrace.TabIndex = 0;
            this.lsbSECSTrace.MouseEnter += new System.EventHandler(this.ListBoxMouseEnter);
            this.lsbSECSTrace.MouseLeave += new System.EventHandler(this.ListBoxMouseLeave);
            // 
            // tbpTool
            // 
            this.tbpTool.Controls.Add(this.tlpTool);
            this.tbpTool.Location = new System.Drawing.Point(4, 26);
            this.tbpTool.Name = "tbpTool";
            this.tbpTool.Padding = new System.Windows.Forms.Padding(3);
            this.tbpTool.Size = new System.Drawing.Size(872, 257);
            this.tbpTool.TabIndex = 5;
            this.tbpTool.Text = "Tool";
            this.tbpTool.UseVisualStyleBackColor = true;
            // 
            // tlpTool
            // 
            this.tlpTool.ColumnCount = 8;
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpTool.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpTool.Controls.Add(this.butRestartSECSDriver, 0, 0);
            this.tlpTool.Controls.Add(this.butSenS1F1_Test, 0, 2);
            this.tlpTool.Controls.Add(this.butEnableCommunication_Test, 0, 3);
            this.tlpTool.Controls.Add(this.butOnLineRemote_Test, 2, 0);
            this.tlpTool.Controls.Add(this.butAuto, 4, 0);
            this.tlpTool.Controls.Add(this.butPause, 4, 1);
            this.tlpTool.Controls.Add(this.butOffLine_Test, 2, 2);
            this.tlpTool.Controls.Add(this.butOnLineLocal_Test, 2, 1);
            this.tlpTool.Controls.Add(this.butSTKC, 6, 0);
            this.tlpTool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTool.Location = new System.Drawing.Point(3, 3);
            this.tlpTool.Name = "tlpTool";
            this.tlpTool.RowCount = 8;
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTool.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlpTool.Size = new System.Drawing.Size(866, 251);
            this.tlpTool.TabIndex = 29;
            // 
            // butRestartSECSDriver
            // 
            this.butRestartSECSDriver.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butRestartSECSDriver.Location = new System.Drawing.Point(3, 3);
            this.butRestartSECSDriver.Name = "butRestartSECSDriver";
            this.butRestartSECSDriver.Size = new System.Drawing.Size(69, 24);
            this.butRestartSECSDriver.TabIndex = 21;
            this.butRestartSECSDriver.Text = "Restart Driver";
            this.butRestartSECSDriver.UseVisualStyleBackColor = true;
            this.butRestartSECSDriver.Click += new System.EventHandler(this.butRestartSECSDriver_Click);
            // 
            // butSenS1F1_Test
            // 
            this.butSenS1F1_Test.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butSenS1F1_Test.Location = new System.Drawing.Point(3, 63);
            this.butSenS1F1_Test.Name = "butSenS1F1_Test";
            this.butSenS1F1_Test.Size = new System.Drawing.Size(69, 24);
            this.butSenS1F1_Test.TabIndex = 10;
            this.butSenS1F1_Test.Text = "S1F1";
            this.butSenS1F1_Test.UseVisualStyleBackColor = true;
            this.butSenS1F1_Test.Click += new System.EventHandler(this.butSenS1F1_Test_Click);
            // 
            // butEnableCommunication_Test
            // 
            this.butEnableCommunication_Test.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butEnableCommunication_Test.Location = new System.Drawing.Point(3, 93);
            this.butEnableCommunication_Test.Name = "butEnableCommunication_Test";
            this.butEnableCommunication_Test.Size = new System.Drawing.Size(69, 24);
            this.butEnableCommunication_Test.TabIndex = 10;
            this.butEnableCommunication_Test.Text = "S1F13";
            this.butEnableCommunication_Test.UseVisualStyleBackColor = true;
            this.butEnableCommunication_Test.Click += new System.EventHandler(this.butEnableCommunication_Test_Click);
            // 
            // butOnLineRemote_Test
            // 
            this.butOnLineRemote_Test.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butOnLineRemote_Test.Location = new System.Drawing.Point(83, 3);
            this.butOnLineRemote_Test.Name = "butOnLineRemote_Test";
            this.butOnLineRemote_Test.Size = new System.Drawing.Size(69, 24);
            this.butOnLineRemote_Test.TabIndex = 10;
            this.butOnLineRemote_Test.Text = "Remote";
            this.butOnLineRemote_Test.UseVisualStyleBackColor = true;
            this.butOnLineRemote_Test.Click += new System.EventHandler(this.butOnLineRemote_Test_Click);
            // 
            // butAuto
            // 
            this.butAuto.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butAuto.Location = new System.Drawing.Point(163, 3);
            this.butAuto.Name = "butAuto";
            this.butAuto.Size = new System.Drawing.Size(69, 24);
            this.butAuto.TabIndex = 10;
            this.butAuto.Text = "Auto";
            this.butAuto.UseVisualStyleBackColor = true;
            this.butAuto.Click += new System.EventHandler(this.butAuto_Click);
            // 
            // butPause
            // 
            this.butPause.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butPause.Location = new System.Drawing.Point(163, 33);
            this.butPause.Name = "butPause";
            this.butPause.Size = new System.Drawing.Size(69, 24);
            this.butPause.TabIndex = 10;
            this.butPause.Text = "Pause";
            this.butPause.UseVisualStyleBackColor = true;
            this.butPause.Click += new System.EventHandler(this.butPause_Click);
            // 
            // butOffLine_Test
            // 
            this.butOffLine_Test.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butOffLine_Test.Location = new System.Drawing.Point(83, 63);
            this.butOffLine_Test.Name = "butOffLine_Test";
            this.butOffLine_Test.Size = new System.Drawing.Size(69, 24);
            this.butOffLine_Test.TabIndex = 10;
            this.butOffLine_Test.Text = "OffLine";
            this.butOffLine_Test.UseVisualStyleBackColor = true;
            this.butOffLine_Test.Click += new System.EventHandler(this.butOffLine_Test_Click);
            // 
            // butOnLineLocal_Test
            // 
            this.butOnLineLocal_Test.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.butOnLineLocal_Test.Location = new System.Drawing.Point(83, 33);
            this.butOnLineLocal_Test.Name = "butOnLineLocal_Test";
            this.butOnLineLocal_Test.Size = new System.Drawing.Size(69, 24);
            this.butOnLineLocal_Test.TabIndex = 10;
            this.butOnLineLocal_Test.Text = "Local";
            this.butOnLineLocal_Test.UseVisualStyleBackColor = true;
            this.butOnLineLocal_Test.Click += new System.EventHandler(this.butOnLineLocal_Test_Click);
            // 
            // butSTKC
            // 
            this.butSTKC.Location = new System.Drawing.Point(243, 3);
            this.butSTKC.Name = "butSTKC";
            this.butSTKC.Size = new System.Drawing.Size(94, 24);
            this.butSTKC.TabIndex = 22;
            this.butSTKC.Text = "STKC View";
            this.butSTKC.UseVisualStyleBackColor = true;
            this.butSTKC.Click += new System.EventHandler(this.butSTKC_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tabReason);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(872, 257);
            this.tabPage1.TabIndex = 6;
            this.tabPage1.Text = "Can\'t Execute Reason";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // timerUIRefresh
            // 
            this.timerUIRefresh.Interval = 500;
            this.timerUIRefresh.Tick += new System.EventHandler(this.timerUIRefresh_Tick);
            // 
            // nicTaskAgent
            // 
            this.nicTaskAgent.ContextMenuStrip = this.cmsShowIcon;
            this.nicTaskAgent.Icon = ((System.Drawing.Icon)(resources.GetObject("nicTaskAgent.Icon")));
            this.nicTaskAgent.Text = "Mirle TaskAgent";
            this.nicTaskAgent.Visible = true;
            // 
            // cmsShowIcon
            // 
            this.cmsShowIcon.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsShowIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShow,
            this.toolStripSeparator1,
            this.tsmiClose});
            this.cmsShowIcon.Name = "cmsShowIcon";
            this.cmsShowIcon.Size = new System.Drawing.Size(168, 54);
            this.cmsShowIcon.DoubleClick += new System.EventHandler(this.cmsShowIcon_DoubleClick);
            // 
            // tsmiShow
            // 
            this.tsmiShow.Name = "tsmiShow";
            this.tsmiShow.Size = new System.Drawing.Size(167, 22);
            this.tsmiShow.Text = "Show TaskAgent";
            this.tsmiShow.Click += new System.EventHandler(this.tsmiShow_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // tsmiClose
            // 
            this.tsmiClose.Name = "tsmiClose";
            this.tsmiClose.Size = new System.Drawing.Size(167, 22);
            this.tsmiClose.Text = "Close TaskAgent";
            this.tsmiClose.Click += new System.EventHandler(this.tsmiClose_Click);
            // 
            // tabReason
            // 
            this.tabReason.Controls.Add(this.tbpTaskk);
            this.tabReason.Controls.Add(this.tbpUpdate);
            this.tabReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabReason.Location = new System.Drawing.Point(3, 3);
            this.tabReason.Name = "tabReason";
            this.tabReason.SelectedIndex = 0;
            this.tabReason.Size = new System.Drawing.Size(866, 251);
            this.tabReason.TabIndex = 2;
            // 
            // tbpTaskk
            // 
            this.tbpTaskk.Controls.Add(this.lsbTaskReason);
            this.tbpTaskk.Location = new System.Drawing.Point(4, 26);
            this.tbpTaskk.Name = "tbpTaskk";
            this.tbpTaskk.Padding = new System.Windows.Forms.Padding(3);
            this.tbpTaskk.Size = new System.Drawing.Size(858, 221);
            this.tbpTaskk.TabIndex = 0;
            this.tbpTaskk.Text = "Task";
            this.tbpTaskk.UseVisualStyleBackColor = true;
            // 
            // lsbTaskReason
            // 
            this.lsbTaskReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbTaskReason.FormattingEnabled = true;
            this.lsbTaskReason.ItemHeight = 17;
            this.lsbTaskReason.Location = new System.Drawing.Point(3, 3);
            this.lsbTaskReason.Name = "lsbTaskReason";
            this.lsbTaskReason.Size = new System.Drawing.Size(852, 215);
            this.lsbTaskReason.TabIndex = 0;
            // 
            // tbpUpdate
            // 
            this.tbpUpdate.Controls.Add(this.lsbUpdateReason);
            this.tbpUpdate.Location = new System.Drawing.Point(4, 26);
            this.tbpUpdate.Name = "tbpUpdate";
            this.tbpUpdate.Padding = new System.Windows.Forms.Padding(3);
            this.tbpUpdate.Size = new System.Drawing.Size(858, 221);
            this.tbpUpdate.TabIndex = 1;
            this.tbpUpdate.Text = "Update";
            this.tbpUpdate.UseVisualStyleBackColor = true;
            // 
            // lsbUpdateReason
            // 
            this.lsbUpdateReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbUpdateReason.FormattingEnabled = true;
            this.lsbUpdateReason.ItemHeight = 17;
            this.lsbUpdateReason.Location = new System.Drawing.Point(3, 3);
            this.lsbUpdateReason.Name = "lsbUpdateReason";
            this.lsbUpdateReason.Size = new System.Drawing.Size(852, 215);
            this.lsbUpdateReason.TabIndex = 1;
            // 
            // TaskAgent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(984, 361);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 400);
            this.MinimumSize = new System.Drawing.Size(1000, 400);
            this.Name = "TaskAgent";
            this.Text = "Mirle Stocker TaskAgent";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TaskAgent_FormClosing);
            this.Load += new System.EventHandler(this.Agent_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TaskAgent_KeyDown);
            this.Resize += new System.EventHandler(this.TaskAgent_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tlpStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tbpTransfer.ResumeLayout(false);
            this.tbpSysTrace.ResumeLayout(false);
            this.tbpSECSTrace.ResumeLayout(false);
            this.tbpTool.ResumeLayout(false);
            this.tlpTool.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.cmsShowIcon.ResumeLayout(false);
            this.tabReason.ResumeLayout(false);
            this.tbpTaskk.ResumeLayout(false);
            this.tbpUpdate.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tlpStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblNowDateTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblSTKID;
        private System.Windows.Forms.TableLayoutPanel tlpTool;
        private System.Windows.Forms.Button butRestartSECSDriver;
        private System.Windows.Forms.Button butSenS1F1_Test;
        private System.Windows.Forms.Button butEnableCommunication_Test;
        private System.Windows.Forms.Button butAuto;
        private System.Windows.Forms.Button butPause;
        private System.Windows.Forms.Button butOnLineRemote_Test;
        private System.Windows.Forms.Button butOffLine_Test;
        private System.Windows.Forms.TabControl tbpTransfer;
        private System.Windows.Forms.TabPage tbpSysTrace;
        private System.Windows.Forms.TabPage tbpSECSTrace;
        private System.Windows.Forms.TabPage tbpTool;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSECSPort;
        private System.Windows.Forms.Label lblSCState;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblSECSIP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblControlMode;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblCommunicationState;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Timer timerUIRefresh;
        private System.Windows.Forms.ListBox lsbTaskTrace;
        private System.Windows.Forms.ListBox lsbSECSTrace;
        private System.Windows.Forms.Button butExitProgram;
        private System.Windows.Forms.NotifyIcon nicTaskAgent;
        private System.Windows.Forms.ContextMenuStrip cmsShowIcon;
        private System.Windows.Forms.ToolStripMenuItem tsmiShow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiClose;
        private System.Windows.Forms.Button butOnLineLocal_Test;
        private System.Windows.Forms.Button butSTKC;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabReason;
        private System.Windows.Forms.TabPage tbpTaskk;
        private System.Windows.Forms.ListBox lsbTaskReason;
        private System.Windows.Forms.TabPage tbpUpdate;
        private System.Windows.Forms.ListBox lsbUpdateReason;
    }
}