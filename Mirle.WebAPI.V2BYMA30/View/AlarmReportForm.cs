using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Mirle.Def;
using System.Linq;
using Mirle.WebAPI.V2BYMA30.Function;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using System.Windows.Forms;

namespace Mirle.WebAPI.V2BYMA30.View
{
    public partial class AlarmReportForm : Form
    {
        private WebApiConfig _config = new WebApiConfig();
        private ALARM_HAPPEN_REPORT alarmReport;
        public AlarmReportForm(WebApiConfig Config)
        {
            _config = Config;
            alarmReport = new ALARM_HAPPEN_REPORT(_config);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            ALARM_HAPPEN_REPORTInfo info = new ALARM_HAPPEN_REPORTInfo
            {
                jobId = txtJobID.Text,
                transactionId = "ALARM_HAPPEN_REPORT",
                deviceId = txtDeviceID.Text,
                alarmCode = txtAlarmID.Text,
                alarmDef = txtAlarmName.Text,
                bufferId = txtLocation.Text,
                status = txtStatus.Text,
                happenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };


            alarmReport.FunReport(info);
            button1.Enabled = true;
        }
    }
}
