namespace Mirle.ASRS.Conveyor.V2BYMA30_8F.View
{
    partial class uclBuffer
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
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblBufferName = new System.Windows.Forms.Label();
            this.lblCmdSno = new System.Windows.Forms.Label();
            this.lblReadNotice = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.lblDone = new System.Windows.Forms.Label();
            this.lblAuto = new System.Windows.Forms.Label();
            this.lblInitialNotice = new System.Windows.Forms.Label();
            this.lblPathNotice = new System.Windows.Forms.Label();
            this.btnBuffer = new System.Windows.Forms.Button();
            this.tlpBuffer = new System.Windows.Forms.TableLayoutPanel();
            this.lblCmdMode = new System.Windows.Forms.Label();
            this.lblLoad = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.lblReady = new System.Windows.Forms.Label();
            this.tlpBuffer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBufferName
            // 
            this.lblBufferName.BackColor = System.Drawing.Color.Green;
            this.lblBufferName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpBuffer.SetColumnSpan(this.lblBufferName, 4);
            this.lblBufferName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBufferName.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblBufferName.ForeColor = System.Drawing.Color.White;
            this.lblBufferName.Location = new System.Drawing.Point(0, 0);
            this.lblBufferName.Margin = new System.Windows.Forms.Padding(0);
            this.lblBufferName.Name = "lblBufferName";
            this.lblBufferName.Size = new System.Drawing.Size(71, 18);
            this.lblBufferName.TabIndex = 480;
            this.lblBufferName.Text = "BufName";
            this.lblBufferName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBufferName.Click += new System.EventHandler(this.uclBuffer_Click);
            // 
            // lblCmdSno
            // 
            this.lblCmdSno.BackColor = System.Drawing.Color.White;
            this.lblCmdSno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tlpBuffer.SetColumnSpan(this.lblCmdSno, 2);
            this.lblCmdSno.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCmdSno.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCmdSno.Location = new System.Drawing.Point(0, 18);
            this.lblCmdSno.Margin = new System.Windows.Forms.Padding(0);
            this.lblCmdSno.Name = "lblCmdSno";
            this.lblCmdSno.Size = new System.Drawing.Size(40, 18);
            this.lblCmdSno.TabIndex = 473;
            this.lblCmdSno.Text = "12345";
            this.lblCmdSno.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCmdSno.Click += new System.EventHandler(this.uclBuffer_Click);
            // 
            // lblReadNotice
            // 
            this.lblReadNotice.BackColor = System.Drawing.Color.MintCream;
            this.lblReadNotice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReadNotice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReadNotice.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblReadNotice.Location = new System.Drawing.Point(0, 36);
            this.lblReadNotice.Margin = new System.Windows.Forms.Padding(0);
            this.lblReadNotice.Name = "lblReadNotice";
            this.lblReadNotice.Size = new System.Drawing.Size(18, 18);
            this.lblReadNotice.TabIndex = 474;
            this.lblReadNotice.Text = "0";
            this.lblReadNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblReadNotice.Click += new System.EventHandler(this.uclBuffer_Click);
            // 
            // lblPosition
            // 
            this.lblPosition.BackColor = System.Drawing.Color.White;
            this.lblPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPosition.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblPosition.Location = new System.Drawing.Point(0, 54);
            this.lblPosition.Margin = new System.Windows.Forms.Padding(0);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(18, 19);
            this.lblPosition.TabIndex = 476;
            this.lblPosition.Text = "0";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPosition.Click += new System.EventHandler(this.uclBuffer_Click);
            // 
            // lblDone
            // 
            this.lblDone.BackColor = System.Drawing.Color.White;
            this.lblDone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDone.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDone.Location = new System.Drawing.Point(18, 54);
            this.lblDone.Margin = new System.Windows.Forms.Padding(0);
            this.lblDone.Name = "lblDone";
            this.lblDone.Size = new System.Drawing.Size(22, 19);
            this.lblDone.TabIndex = 477;
            this.lblDone.Text = "0";
            this.lblDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDone.Click += new System.EventHandler(this.uclBuffer_Click);
            // 
            // lblAuto
            // 
            this.lblAuto.BackColor = System.Drawing.Color.White;
            this.lblAuto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAuto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAuto.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAuto.Location = new System.Drawing.Point(40, 54);
            this.lblAuto.Margin = new System.Windows.Forms.Padding(0);
            this.lblAuto.Name = "lblAuto";
            this.lblAuto.Size = new System.Drawing.Size(14, 19);
            this.lblAuto.TabIndex = 478;
            this.lblAuto.Text = "0";
            this.lblAuto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAuto.Click += new System.EventHandler(this.uclBuffer_Click);
            // 
            // lblInitialNotice
            // 
            this.lblInitialNotice.BackColor = System.Drawing.Color.White;
            this.lblInitialNotice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblInitialNotice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInitialNotice.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblInitialNotice.Location = new System.Drawing.Point(40, 36);
            this.lblInitialNotice.Margin = new System.Windows.Forms.Padding(0);
            this.lblInitialNotice.Name = "lblInitialNotice";
            this.lblInitialNotice.Size = new System.Drawing.Size(14, 18);
            this.lblInitialNotice.TabIndex = 482;
            this.lblInitialNotice.Text = "0";
            this.lblInitialNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPathNotice
            // 
            this.lblPathNotice.BackColor = System.Drawing.Color.White;
            this.lblPathNotice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPathNotice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPathNotice.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblPathNotice.Location = new System.Drawing.Point(18, 36);
            this.lblPathNotice.Margin = new System.Windows.Forms.Padding(0);
            this.lblPathNotice.Name = "lblPathNotice";
            this.lblPathNotice.Size = new System.Drawing.Size(22, 18);
            this.lblPathNotice.TabIndex = 481;
            this.lblPathNotice.Text = "0";
            this.lblPathNotice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBuffer
            // 
            this.btnBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBuffer.Location = new System.Drawing.Point(0, 0);
            this.btnBuffer.Name = "btnBuffer";
            this.btnBuffer.Padding = new System.Windows.Forms.Padding(3);
            this.btnBuffer.Size = new System.Drawing.Size(71, 73);
            this.btnBuffer.TabIndex = 483;
            this.btnBuffer.UseVisualStyleBackColor = true;
            // 
            // tlpBuffer
            // 
            this.tlpBuffer.AllowDrop = true;
            this.tlpBuffer.BackColor = System.Drawing.Color.White;
            this.tlpBuffer.ColumnCount = 4;
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21F));
            this.tlpBuffer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21F));
            this.tlpBuffer.Controls.Add(this.lblCmdMode, 2, 1);
            this.tlpBuffer.Controls.Add(this.lblLoad, 3, 3);
            this.tlpBuffer.Controls.Add(this.lblError, 3, 2);
            this.tlpBuffer.Controls.Add(this.lblReady, 3, 1);
            this.tlpBuffer.Controls.Add(this.lblPosition, 0, 3);
            this.tlpBuffer.Controls.Add(this.lblDone, 1, 3);
            this.tlpBuffer.Controls.Add(this.lblAuto, 2, 3);
            this.tlpBuffer.Controls.Add(this.lblReadNotice, 0, 2);
            this.tlpBuffer.Controls.Add(this.lblPathNotice, 1, 2);
            this.tlpBuffer.Controls.Add(this.lblInitialNotice, 2, 2);
            this.tlpBuffer.Controls.Add(this.lblBufferName, 0, 0);
            this.tlpBuffer.Controls.Add(this.lblCmdSno, 0, 1);
            this.tlpBuffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBuffer.Location = new System.Drawing.Point(0, 0);
            this.tlpBuffer.Name = "tlpBuffer";
            this.tlpBuffer.RowCount = 4;
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBuffer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpBuffer.Size = new System.Drawing.Size(71, 73);
            this.tlpBuffer.TabIndex = 2;
            this.tlpBuffer.Click += new System.EventHandler(this.uclBuffer_Click);
            // 
            // lblCmdMode
            // 
            this.lblCmdMode.BackColor = System.Drawing.Color.White;
            this.lblCmdMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCmdMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCmdMode.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCmdMode.Location = new System.Drawing.Point(40, 18);
            this.lblCmdMode.Margin = new System.Windows.Forms.Padding(0);
            this.lblCmdMode.Name = "lblCmdMode";
            this.lblCmdMode.Size = new System.Drawing.Size(14, 18);
            this.lblCmdMode.TabIndex = 485;
            this.lblCmdMode.Text = "0";
            this.lblCmdMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoad
            // 
            this.lblLoad.BackColor = System.Drawing.Color.LightGreen;
            this.lblLoad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLoad.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblLoad.Location = new System.Drawing.Point(54, 54);
            this.lblLoad.Margin = new System.Windows.Forms.Padding(0);
            this.lblLoad.Name = "lblLoad";
            this.lblLoad.Size = new System.Drawing.Size(17, 19);
            this.lblLoad.TabIndex = 484;
            this.lblLoad.Text = "0";
            this.lblLoad.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblError
            // 
            this.lblError.BackColor = System.Drawing.Color.Snow;
            this.lblError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblError.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblError.Location = new System.Drawing.Point(54, 36);
            this.lblError.Margin = new System.Windows.Forms.Padding(0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(17, 18);
            this.lblError.TabIndex = 484;
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblReady
            // 
            this.lblReady.BackColor = System.Drawing.Color.White;
            this.lblReady.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReady.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReady.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblReady.Location = new System.Drawing.Point(54, 18);
            this.lblReady.Margin = new System.Windows.Forms.Padding(0);
            this.lblReady.Name = "lblReady";
            this.lblReady.Size = new System.Drawing.Size(17, 18);
            this.lblReady.TabIndex = 484;
            this.lblReady.Text = "0";
            this.lblReady.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uclBuffer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tlpBuffer);
            this.Controls.Add(this.btnBuffer);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uclBuffer";
            this.Size = new System.Drawing.Size(71, 73);
            this.Click += new System.EventHandler(this.uclBuffer_Click);
            this.tlpBuffer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblBufferName;
        private System.Windows.Forms.Label lblCmdSno;
        private System.Windows.Forms.Label lblReadNotice;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblDone;
        private System.Windows.Forms.Label lblAuto;
        private System.Windows.Forms.Label lblInitialNotice;
        private System.Windows.Forms.Label lblPathNotice;
        private System.Windows.Forms.Button btnBuffer;
        private System.Windows.Forms.TableLayoutPanel tlpBuffer;
        private System.Windows.Forms.Label lblLoad;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblReady;
        private System.Windows.Forms.Label lblCmdMode;
    }
}
