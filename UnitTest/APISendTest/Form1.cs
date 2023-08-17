using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;
using Mirle;
using Mirle.WebAPI.V2BYMA30.Function;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Mirle.Def;
using APISendTest.WebAPI;

namespace APISendTest
{
    public partial class Form1 : Form
    {
        private UnityContainer _unityContainer = new UnityContainer();
        private Host _webApiHost;
        private WebApiConfig send_webApiConfig = new WebApiConfig
        {
            //IP = "172.18.135.4:9001"
            IP = "127.0.0.1:9000"          
        };

        private CV_RECEIVE_NEW_BIN_CMD cv_RECEIVE_NEW_BIN_CMD;
        private CV_WRITE_CMD writeCV;
        private BUFFER_ROLL_INFO cv_rolling;
        private BUFFER_STATUS_QUERY buffer_status_query;
        private CV_BUFFER_QUERY cv_buffer_query; 
        private EMPTY_BIN_LOAD_DONE empty_BIN_LOAD_DONE;
        private ALARM_HAPPEN_REPORT alarm_happen_report;

        //private EmptyShelfQuery emptyShelfQuery;
        //private string listening_ip = "*:9000";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _unityContainer.RegisterInstance(new TestController());
            //_webApiHost = new Host(new Startup(_unityContainer), listening_ip);

            //emptyShelfQuery = new EmptyShelfQuery(send_webApiConfig);
            cv_RECEIVE_NEW_BIN_CMD = new CV_RECEIVE_NEW_BIN_CMD(send_webApiConfig);
            writeCV = new CV_WRITE_CMD(send_webApiConfig);
            cv_rolling = new BUFFER_ROLL_INFO(send_webApiConfig);
            buffer_status_query = new BUFFER_STATUS_QUERY(send_webApiConfig);
            empty_BIN_LOAD_DONE = new EMPTY_BIN_LOAD_DONE(send_webApiConfig);

            alarm_happen_report = new ALARM_HAPPEN_REPORT(send_webApiConfig);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EmptyShelfQuery_TEST();
        }

        public void EmptyShelfQuery_TEST()
        {
            //EmptyShelfQuery_WCS request = new EmptyShelfQuery_WCS
            //{
            //    jobId = textBox1.Text,
            //    transactionId = textBox2.Text,
            //    carrierId = textBox3.Text
            //};
            //EmptyShelfQuery_WMS response = new EmptyShelfQuery_WMS();

            //emptyShelfQuery.funReport(request, ref response);

            return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CV_RECEIVE_NEW_BIN_CMDInfo_WCS request = new CV_RECEIVE_NEW_BIN_CMDInfo_WCS
            {
                jobId = textBox1.Text,
                transactionId = "CV_RECEIVE_NEW_BIN_CMD ",
                bufferId = textBox2.Text,
                carrierType = textBox4.Text
            };
            CV_RECEIVE_NEW_BIN_CMDInfo_WMS response = new CV_RECEIVE_NEW_BIN_CMDInfo_WMS();
            cv_RECEIVE_NEW_BIN_CMD.funReport(request, ref response);

            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EMPTY_BIN_LOAD_DONEInfo_WCS request = new EMPTY_BIN_LOAD_DONEInfo_WCS
            {
                jobId = textBox1.Text,
                transactionId = "EMPTY_BIN_LOAD_DONE ", 
                location = textBox2.Text
                
            };
            EMPTY_BIN_LOAD_DONEInfo_WMS response = new EMPTY_BIN_LOAD_DONEInfo_WMS();
            empty_BIN_LOAD_DONE.funReport(request, ref response);

            return;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //CV_WRITE_CMDInfo request = new CV_WRITE_CMDInfo
            //{
            //    jobId = textBox1.Text,
            //    transactionId = "CV_WRITE_CMD ",
            //    bufferId = textBox2.Text,
            //    toLocation = textBox3.Text
            //};
            //CV_WRITE_CMDInfo_Response response = new CV_WRITE_CMDInfo_Response();
            //writeCV.FunReport(request);

            return;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BUFFER_ROLL_INFO_WCS request = new BUFFER_ROLL_INFO_WCS
            {
                jobId = textBox1.Text,
                transactionId = "BUFFER_ROLL_INFO ",
                bufferId = textBox2.Text
            };
            BUFFER_ROLL_INFO_WMS response = new BUFFER_ROLL_INFO_WMS();
            cv_rolling.funReport(request, ref response);

            return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BUFFER_STATUS_QUERYInfo request = new BUFFER_STATUS_QUERYInfo
            {
                jobId = textBox1.Text,
                transactionId = "BUFFER_STATUS_QUERY ",
                bufferId = textBox2.Text
            };
            BUFFER_STATUS_QUERYInfo_Response response = new BUFFER_STATUS_QUERYInfo_Response();

            buffer_status_query.FunReport(request, ref response);

            return;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ALARM_HAPPEN_REPORTInfo request = new ALARM_HAPPEN_REPORTInfo()
            {
                jobId = "123",
                deviceId = "LO2",
                alarmCode = "0070",
                alarmDef = "10F",
                bufferId = "test",
                status = "0",
                happenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                //happenTime = "2023-06-20 14:28:00"
            };

            alarm_happen_report.FunReport(request);

            return;
        }
    }
}
