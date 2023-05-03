using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RetrieveTransferInfo info = new RetrieveTransferInfo
            {
                jobId = textBox1.Text,
                carrierId = textBox2.Text,
                fromShelfId = textBox3.Text,
                toPortId = textBox4.Text,
                backupPortId = textBox5.Text,
                priority = textBox6.Text,
                batchId = textBox7.Text
            };

            FunReport(info);
        }

        public bool FunReport(RetrieveTransferInfo info)
        {
            string strJson = Newtonsoft.Json.JsonConvert.SerializeObject(info);
            //clsWriLog.Log.FunWriTraceLog_CV(strJson);
            string sLink = $"http://127.0.0.1:9000/WCS/RetrieveTransfer";
            //string strResonse = HttpPost(sLink, strJson);

            var add2 = $"http://127.0.0.1:9000/WCS/RETRIEVE_TRANSFER";
            Task.Run(() =>
            {
                using (HttpClient client = new HttpClient())
                {
                    var result2 = client.PostAsJsonAsync(add2, info).Result;
                }
            });

            return true;
        }
        //public string HttpPost(string url, RetrieveTransferInfo body)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        #region 呼叫遠端 Web API
        //        HttpResponseMessage response = null;

        //        #region  設定相關網址內容
        //        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
        //        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        // Content-Type 用於宣告遞送給對方的文件型態
        //        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        //        var fooFullUrl = $"{url}";
        //        using (var fooContent = new StringContent(body, Encoding.UTF8, "application/json"))
        //        {
        //            response = client.PostAsync(fooFullUrl, fooContent).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                return response.Content.ReadAsStringAsync().Result;
        //            }
        //            else if (response != null)
        //            {
        //                throw new Exception(response.Content.ReadAsStringAsync().Result);
        //            }
        //            else
        //                return "";
        //        }
        //        #endregion
        //        #endregion
        //    }
        //}
    }
}
