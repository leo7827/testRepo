using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.Function;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using System.Windows.Forms;

namespace Mirle.WebAPI.V2BYMA30.View
{
    public partial class PositionReportForm : Form
    {
        private PositionReport positionReport;
        public PositionReportForm(WebApiConfig config)
        {
            positionReport = new PositionReport(config);
            InitializeComponent();
        }

        private void PositionReportForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            POSITION_REPORTInfo info = new POSITION_REPORTInfo
            {
                jobId = txtJobID.Text,
                transactionId = "POSITION_REPORT",
                carrierType = "Bin",
                id = "",
                position = txtLocation.Text
                
            };

            positionReport.FunReport(info);
            button1.Enabled = true;
        }
    }
}
