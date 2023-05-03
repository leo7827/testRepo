namespace Mirle.ASRS.Conveyor.V2BYMA30_Elevator.MPLCView
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
            this.btnRM2 = new System.Windows.Forms.Button();
            this.btnLayout = new System.Windows.Forms.Button();
            this.childPanel = new System.Windows.Forms.Panel();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnRM2
            // 
            this.btnRM2.Location = new System.Drawing.Point(74, 8);
            this.btnRM2.Name = "btnRM2";
            this.btnRM2.Size = new System.Drawing.Size(92, 21);
            this.btnRM2.TabIndex = 78;
            this.btnRM2.Text = "Buffer PLC Info";
            this.btnRM2.UseVisualStyleBackColor = true;
            this.btnRM2.Click += new System.EventHandler(this.btnRM2_Click);
            // 
            // btnLayout
            // 
            this.btnLayout.Location = new System.Drawing.Point(12, 8);
            this.btnLayout.Name = "btnLayout";
            this.btnLayout.Size = new System.Drawing.Size(49, 21);
            this.btnLayout.TabIndex = 77;
            this.btnLayout.Text = "Layout";
            this.btnLayout.UseVisualStyleBackColor = true;
            this.btnLayout.Click += new System.EventHandler(this.btnLayout_Click);
            // 
            // childPanel
            // 
            this.childPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.childPanel.Location = new System.Drawing.Point(2, 33);
            this.childPanel.Name = "childPanel";
            this.childPanel.Size = new System.Drawing.Size(879, 555);
            this.childPanel.TabIndex = 76;
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Interval = 200;
            // 
            // MonitorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 594);
            this.Controls.Add(this.btnRM2);
            this.Controls.Add(this.btnLayout);
            this.Controls.Add(this.childPanel);
            this.Name = "MonitorMainForm";
            this.Text = "MonitorMainForm";
            this.Load += new System.EventHandler(this.MonitorMainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnRM2;
        private System.Windows.Forms.Button btnLayout;
        private System.Windows.Forms.Panel childPanel;
        private System.Windows.Forms.Timer RefreshTimer;
    }
}