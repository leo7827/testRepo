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
    public partial class CMD_DESTINATION_CHECKForm : Form
    {
        private WebApiConfig _config = new WebApiConfig();
        private CMD_DESTINATION_CHECK cmd_destination_check;
        public CMD_DESTINATION_CHECKForm(WebApiConfig Config)
        {
            _config = Config;
            cmd_destination_check = new CMD_DESTINATION_CHECK(_config);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            CMD_DESTINATION_CHECKInfo info = new CMD_DESTINATION_CHECKInfo
            {
                jobId = txtJobID.Text,
                transactionId = "CMD_DESTINATION_CHECK",
                location = txtLocation.Text
            };

            CMD_DESTINATION_CHECKInfo_Response response = new CMD_DESTINATION_CHECKInfo_Response();
            cmd_destination_check.funReport(info, ref response);
            button1.Enabled = true;
        }
    }
}
