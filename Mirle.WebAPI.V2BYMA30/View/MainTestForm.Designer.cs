namespace Mirle.WebAPI.V2BYMA30.View
{
    partial class MainTestForm
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
            this.btnBCR_CHECK_REQUEST = new System.Windows.Forms.Button();
            this.btnPickupQuery = new System.Windows.Forms.Button();
            this.btnPositionReport = new System.Windows.Forms.Button();
            this.btnPutawayCheck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBCR_CHECK_REQUEST
            // 
            this.btnBCR_CHECK_REQUEST.Location = new System.Drawing.Point(40, 24);
            this.btnBCR_CHECK_REQUEST.Name = "btnBCR_CHECK_REQUEST";
            this.btnBCR_CHECK_REQUEST.Size = new System.Drawing.Size(178, 23);
            this.btnBCR_CHECK_REQUEST.TabIndex = 0;
            this.btnBCR_CHECK_REQUEST.Text = "BCR_CHECK_REQUEST";
            this.btnBCR_CHECK_REQUEST.UseVisualStyleBackColor = true;
            this.btnBCR_CHECK_REQUEST.Click += new System.EventHandler(this.btnBCR_CHECK_REQUEST_Click);
            // 
            // btnPickupQuery
            // 
            this.btnPickupQuery.Location = new System.Drawing.Point(40, 115);
            this.btnPickupQuery.Name = "btnPickupQuery";
            this.btnPickupQuery.Size = new System.Drawing.Size(100, 23);
            this.btnPickupQuery.TabIndex = 1;
            this.btnPickupQuery.Text = "AlarmReport";
            this.btnPickupQuery.UseVisualStyleBackColor = true;
            this.btnPickupQuery.Click += new System.EventHandler(this.btnAlarmReport_Click);
            // 
            // btnPositionReport
            // 
            this.btnPositionReport.Location = new System.Drawing.Point(40, 71);
            this.btnPositionReport.Name = "btnPositionReport";
            this.btnPositionReport.Size = new System.Drawing.Size(100, 23);
            this.btnPositionReport.TabIndex = 2;
            this.btnPositionReport.Text = "PositionReport";
            this.btnPositionReport.UseVisualStyleBackColor = true;
            this.btnPositionReport.Click += new System.EventHandler(this.btnPositionReport_Click);
            // 
            // btnPutawayCheck
            // 
            this.btnPutawayCheck.Location = new System.Drawing.Point(40, 169);
            this.btnPutawayCheck.Name = "btnPutawayCheck";
            this.btnPutawayCheck.Size = new System.Drawing.Size(100, 23);
            this.btnPutawayCheck.TabIndex = 3;
            this.btnPutawayCheck.Text = "CMD_DESTINATION_CHECK";
            this.btnPutawayCheck.UseVisualStyleBackColor = true;
            this.btnPutawayCheck.Click += new System.EventHandler(this.btnCMD_DESTINATION_CHECK_Click);
            // 
            // MainTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnPutawayCheck);
            this.Controls.Add(this.btnPositionReport);
            this.Controls.Add(this.btnPickupQuery);
            this.Controls.Add(this.btnBCR_CHECK_REQUEST);
            this.Name = "MainTestForm";
            this.Text = "MainTestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBCR_CHECK_REQUEST;
        private System.Windows.Forms.Button btnPickupQuery;
        private System.Windows.Forms.Button btnPositionReport;
        private System.Windows.Forms.Button btnPutawayCheck;
    }
}