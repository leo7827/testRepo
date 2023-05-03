using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirle.Def;
using Mirle.STKC.R46YP320;
using Mirle.STKC.R46YP320.Model;
using Mirle.STKC.R46YP320.ViewModels;

namespace STKCTest
{
    public partial class Form1 : Form
    {
        private readonly System.Timers.Timer _timer = new System.Timers.Timer();
        private readonly STKCHost stkcHost;

        public Form1()
        {
            InitializeComponent();
            stkcHost = new STKCHost();
            stkcHost.GetMainView().Show();

            cbbForkNumber.Items.Clear();
            cbbForkNumber.Items.Add("1");
            cbbForkNumber.Items.Add("2");
            cbbForkNumber.SelectedIndex = 0;
            cbbTransferMode.Items.Clear();
            cbbTransferMode.Items.Add("1-MOVE");
            cbbTransferMode.Items.Add("2-FROM");
            cbbTransferMode.Items.Add("3-TO");
            cbbTransferMode.Items.Add("4-FROM_TO");
            cbbTransferMode.SelectedIndex = 0;
            dataGridView1.ColumnCount = 4;

            dataGridView1.Columns[0].Name = "Fork Nnmber";
            dataGridView1.Columns[1].Name = "Transfer Mode";
            dataGridView1.Columns[2].Name = "Source";
            dataGridView1.Columns[3].Name = "Destination";
        }

        private void butSaveCommand_Click(object sender, EventArgs e)
        {
            int.TryParse(cbbTransferMode.Text.Split("-"[0])[0], out var transferModeNo);
            stkcHost.GetCycleRunService().SetCycle(new SCCommand()
            {
                CraneId = 1,
                ForkNumber = Convert.ToInt32(cbbForkNumber.Text),
                TransferMode = (clsEnum.TaskMode)transferModeNo,
                EnableBCRRead = false,
                Source = Convert.ToInt32(txtSource.Text),
                Destination = Convert.ToInt32(txtDestination.Text),
            });
            string ForkNumber = cbbForkNumber.Text.ToString();
            string TransferMode = ((clsEnum.TaskMode)transferModeNo).ToString();
            string Source = Convert.ToInt32(txtSource.Text).ToString();
            string Destination = Convert.ToInt32(txtDestination.Text).ToString();
            dataGridView1.Rows.Add(ForkNumber, TransferMode, Source, Destination);
        }
        private void forkmode(string forknumber, string transferMode, string source, string destination, bool DFork)
        {
            int.TryParse(transferMode.Split("-"[0])[0], out var transferModeNo);

            if (DFork == true && transferModeNo == 4)
            {
                if (Convert.ToInt32(cbbForkNumber.Text) == 1)
                {
                    //fork =1 from
                    stkcHost.GetCycleRunService().SetCycle(new SCCommand()
                    {
                        CraneId = 1,
                        ForkNumber = 1,
                        TransferMode = (clsEnum.TaskMode)2,
                        EnableBCRRead = false,
                        CstId = "",
                        Source = Convert.ToInt32(source),
                        Destination = 0,
                    });
                    string ForkNumber = "1";
                    string TransferMode = ((clsEnum.TaskMode)2).ToString();
                    string Source = source;
                    string Destination = "0";
                    dataGridView1.Rows.Add(ForkNumber, TransferMode, Source, Destination);

                    //fork2 from
                    string Subsource = Convert.ToString((Convert.ToInt32(source.Substring(2, 3)) + 1)).PadLeft(3, '0');
                    source = source.Remove(2, 3);
                    source = source.Insert(2, Subsource);
                    stkcHost.GetCycleRunService().SetCycle(new SCCommand()
                    {
                        CraneId = 1,
                        ForkNumber = 2,
                        TransferMode = (clsEnum.TaskMode)2,
                        EnableBCRRead = false,
                        CstId = "",
                        Source = Convert.ToInt32(source),
                        Destination = 0,
                    });
                    ForkNumber = "2";
                    TransferMode = ((clsEnum.TaskMode)2).ToString();
                    Source = source;
                    Destination = "0";
                    dataGridView1.Rows.Add(ForkNumber, TransferMode, Source, Destination);


                    //fork1 to
                    stkcHost.GetCycleRunService().SetCycle(new SCCommand()
                    {
                        CraneId = 1,
                        ForkNumber = 1,
                        TransferMode = (clsEnum.TaskMode)3,
                        EnableBCRRead = false,
                        CstId = "",
                        Source = 0,
                        Destination = Convert.ToInt32(destination),
                    });
                    ForkNumber = "1";
                    TransferMode = ((clsEnum.TaskMode)3).ToString();
                    Source = "0";
                    Destination = destination;
                    dataGridView1.Rows.Add(ForkNumber, TransferMode, Source, Destination);
                    //fork2 to
                    string Subdestination = Convert.ToString(Convert.ToInt32(destination.Substring(2, 3)) + 1).PadLeft(3, '0');
                    destination = destination.Remove(2, 3);
                    destination = destination.Insert(2, Subdestination);
                    stkcHost.GetCycleRunService().SetCycle(new SCCommand()
                    {
                        CraneId = 1,
                        ForkNumber = 2,
                        TransferMode = (clsEnum.TaskMode)3,
                        EnableBCRRead = false,
                        CstId = "",
                        Source = 0,
                        Destination = Convert.ToInt32(destination),
                    });
                    ForkNumber = "2";
                    TransferMode = ((clsEnum.TaskMode)3).ToString();
                    Source = "0";
                    Destination = destination;
                    dataGridView1.Rows.Add(ForkNumber, TransferMode, Source, Destination);

                }
                else if (Convert.ToInt32(cbbForkNumber.Text) == 2)
                {
                    return;
                }
            }
            else
            {
                stkcHost.GetCycleRunService().SetCycle(new SCCommand()
                {
                    CraneId = 1,
                    ForkNumber = Convert.ToInt32(forknumber),
                    TransferMode = (clsEnum.TaskMode)transferModeNo,
                    EnableBCRRead = false,
                    CstId = "",
                    Source = Convert.ToInt32(source),
                    Destination = Convert.ToInt32(destination),
                });
                string ForkNumber = forknumber;
                string TransferMode = ((clsEnum.TaskMode)transferModeNo).ToString();
                string Source = source;
                string Destination = destination;
                dataGridView1.Rows.Add(ForkNumber, TransferMode, Source, Destination);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stkcHost.GetCycleRunService().Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stkcHost.GetCycleRunService().Stop();
            stkcHost.GetCycleRunService().ClearCycle();
            stkcHost.GetCycleRunService().ClearTask();
            dataGridView1.Rows.Clear();
        }
    }
}
