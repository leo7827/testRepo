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
    public partial class BCR_CHECK_REQUESTForm : Form
    {
        private WebApiConfig _config = new WebApiConfig();
        private BCR_CHECK_REQUEST bcr_checkrequest;
        public BCR_CHECK_REQUESTForm(WebApiConfig Config)
        {
            _config = Config;
            bcr_checkrequest = new BCR_CHECK_REQUEST(_config);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            BCR_CHECK_REQUESTInfo info = new BCR_CHECK_REQUESTInfo
            {
                jobId = txtJobID.Text,
                transactionId = "BCR_CHECK_REQUEST",
                barcode = txtBarcode.Text,
                location = txtLocation.Text
            };

            BCR_CHECK_REQUESTInfo_Response response = new BCR_CHECK_REQUESTInfo_Response();
            bcr_checkrequest.funReport(info, ref response);
            button1.Enabled = true;
        }
    }
}
