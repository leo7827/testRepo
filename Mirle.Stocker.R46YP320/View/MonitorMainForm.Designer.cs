namespace Mirle.Stocker.R46YP320.View
{
    partial class MonitorMainForm
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
            this.childPanel = new System.Windows.Forms.Panel();
            this.btnRM1 = new System.Windows.Forms.Button();
            this.btnRM2 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label51 = new System.Windows.Forms.Label();
            this.lblHandoffArea = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.lblSharedArea = new System.Windows.Forms.Label();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.btnPort = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // childPanel
            // 
            this.childPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.childPanel.Location = new System.Drawing.Point(2, 39);
            this.childPanel.Name = "childPanel";
            this.childPanel.Size = new System.Drawing.Size(879, 601);
            this.childPanel.TabIndex = 0;
            // 
            // btnRM1
            // 
            this.btnRM1.Location = new System.Drawing.Point(12, 12);
            this.btnRM1.Name = "btnRM1";
            this.btnRM1.Size = new System.Drawing.Size(75, 23);
            this.btnRM1.TabIndex = 1;
            this.btnRM1.Text = "RM1";
            this.btnRM1.UseVisualStyleBackColor = true;
            this.btnRM1.Click += new System.EventHandler(this.btnRM1_Click);
            // 
            // btnRM2
            // 
            this.btnRM2.Location = new System.Drawing.Point(93, 12);
            this.btnRM2.Name = "btnRM2";
            this.btnRM2.Size = new System.Drawing.Size(75, 23);
            this.btnRM2.TabIndex = 2;
            this.btnRM2.Text = "RM2";
            this.btnRM2.UseVisualStyleBackColor = true;
            this.btnRM2.Click += new System.EventHandler(this.btnRM2_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label51);
            this.panel2.Controls.Add(this.lblHandoffArea);
            this.panel2.Controls.Add(this.label47);
            this.panel2.Controls.Add(this.lblSharedArea);
            this.panel2.Location = new System.Drawing.Point(347, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(454, 22);
            this.panel2.TabIndex = 69;
            // 
            // label51
            // 
            this.label51.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label51.ForeColor = System.Drawing.Color.Blue;
            this.label51.Location = new System.Drawing.Point(228, 1);
            this.label51.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(154, 20);
            this.label51.TabIndex = 67;
            this.label51.Text = "HandiffArea-Start>End";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHandoffArea
            // 
            this.lblHandoffArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHandoffArea.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHandoffArea.ForeColor = System.Drawing.Color.Blue;
            this.lblHandoffArea.Location = new System.Drawing.Point(383, 1);
            this.lblHandoffArea.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.lblHandoffArea.Name = "lblHandoffArea";
            this.lblHandoffArea.Size = new System.Drawing.Size(67, 19);
            this.lblHandoffArea.TabIndex = 67;
            this.lblHandoffArea.Text = "0>0";
            this.lblHandoffArea.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label47
            // 
            this.label47.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.ForeColor = System.Drawing.Color.Blue;
            this.label47.Location = new System.Drawing.Point(4, 1);
            this.label47.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(154, 20);
            this.label47.TabIndex = 67;
            this.label47.Text = "SharedArea-Start>End";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSharedArea
            // 
            this.lblSharedArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSharedArea.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSharedArea.ForeColor = System.Drawing.Color.Blue;
            this.lblSharedArea.Location = new System.Drawing.Point(158, 1);
            this.lblSharedArea.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.lblSharedArea.Name = "lblSharedArea";
            this.lblSharedArea.Size = new System.Drawing.Size(67, 19);
            this.lblSharedArea.TabIndex = 67;
            this.lblSharedArea.Text = "0>0";
            this.lblSharedArea.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Interval = 200;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTimer_Tick);
            // 
            // btnPort
            // 
            this.btnPort.Location = new System.Drawing.Point(174, 12);
            this.btnPort.Name = "btnPort";
            this.btnPort.Size = new System.Drawing.Size(75, 23);
            this.btnPort.TabIndex = 70;
            this.btnPort.Text = "Port";
            this.btnPort.UseVisualStyleBackColor = true;
            this.btnPort.Click += new System.EventHandler(this.btnPort_Click);
            // 
            // frmMain_TwinFork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 643);
            this.Controls.Add(this.btnPort);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnRM2);
            this.Controls.Add(this.btnRM1);
            this.Controls.Add(this.childPanel);
            this.Name = "frmMain_TwinFork";
            this.Text = "frmTestPanel";
            this.Load += new System.EventHandler(this.MonitorMainForm_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel childPanel;
        private System.Windows.Forms.Button btnRM1;
        private System.Windows.Forms.Button btnRM2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label lblHandoffArea;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label lblSharedArea;
        private System.Windows.Forms.Timer RefreshTimer;
        private System.Windows.Forms.Button btnPort;
    }
}
