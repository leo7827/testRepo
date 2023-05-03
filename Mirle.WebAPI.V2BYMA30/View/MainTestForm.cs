using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Mirle.Def;
using System.Windows.Forms;

namespace Mirle.WebAPI.V2BYMA30.View
{
    public partial class MainTestForm : Form
    {
        private WebApiConfig _config = new WebApiConfig();
        public MainTestForm(WebApiConfig config)
        {
            _config = config;
            InitializeComponent();
        }

        private BCR_CHECK_REQUESTForm bcr_CHECK_REQUEST;
        private void btnBCR_CHECK_REQUEST_Click(object sender, EventArgs e)
        {
            if (bcr_CHECK_REQUEST == null)
            {
                bcr_CHECK_REQUEST = new BCR_CHECK_REQUESTForm(_config);
                bcr_CHECK_REQUEST.TopMost = true;
                bcr_CHECK_REQUEST.FormClosed += new FormClosedEventHandler(funBCR_CHECK_REQUEST_FormClosed);
                bcr_CHECK_REQUEST.Show();
            }
            else
            {
                bcr_CHECK_REQUEST.BringToFront();
            }
        }

        private void funBCR_CHECK_REQUEST_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (bcr_CHECK_REQUEST != null)
                bcr_CHECK_REQUEST = null;
        }

        private AlarmReportForm alarmReport;
        private void btnAlarmReport_Click(object sender, EventArgs e)
        {
            if (alarmReport == null)
            {
                alarmReport = new AlarmReportForm(_config);
                alarmReport.TopMost = true;
                alarmReport.FormClosed += new FormClosedEventHandler(funAlarmReport_FormClosed);
                alarmReport.Show();
            }
            else
            {
                alarmReport.BringToFront();
            }
        }

        private void funAlarmReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (alarmReport != null)
                alarmReport = null;
        }

        private PositionReportForm positionReport;
        private void btnPositionReport_Click(object sender, EventArgs e)
        {
            if (positionReport == null)
            {
                positionReport = new PositionReportForm(_config);
                positionReport.TopMost = true;
                positionReport.FormClosed += new FormClosedEventHandler(funPositionReport_FormClosed);
                positionReport.Show();
            }
            else
            {
                positionReport.BringToFront();
            }
        }

        private void funPositionReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (positionReport != null)
                positionReport = null;
        }

        private CMD_DESTINATION_CHECKForm cmd_DESTINATION_CHECK;
        private void btnCMD_DESTINATION_CHECK_Click(object sender, EventArgs e)
        {
            if (cmd_DESTINATION_CHECK == null)
            {
                cmd_DESTINATION_CHECK = new CMD_DESTINATION_CHECKForm(_config);
                cmd_DESTINATION_CHECK.TopMost = true;
                cmd_DESTINATION_CHECK.FormClosed += new FormClosedEventHandler(funCMD_DESTINATION_CHECK_FormClosed);
                cmd_DESTINATION_CHECK.Show();
            }
            else
            {
                cmd_DESTINATION_CHECK.BringToFront();
            }
        }
        private void funCMD_DESTINATION_CHECK_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cmd_DESTINATION_CHECK != null)
                cmd_DESTINATION_CHECK = null;
        }


         
      
    }
}
