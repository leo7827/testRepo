using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mirle.MPLC;
using Mirle.MPLC.FileData;
using Mirle.MPLC.DataType;
using Mirle.MPLC.DataBlocks;
using Mirle.ASRS.Conveyor.V2BYMA30_1F.Signal;
using Mirle.ASRS.Conveyor.V2BYMA30_1F.MPLCView;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.MPLCView
{
    public partial class MonitorMainForm : Form
    {
        private MonitorLayout frmLayout;
        private BufferPlcInfoView frmBufferInfo;
        private ConveyorController conveyorController;
        public MonitorMainForm(IMPLCProvider mplc)
        {
            InitializeComponent();
            conveyorController = new ConveyorController(mplc);
            frmLayout = new MonitorLayout(conveyorController);
            frmBufferInfo = new BufferPlcInfoView(conveyorController);
        }

        private void MonitorMainForm_Load(object sender, EventArgs e)
        {
            ShowChildForm(frmLayout);
            this.RefreshTimer.Enabled = true;
        }

        private void ShowChildForm(Form child)
        {
            this.childPanel.Controls.Clear();
            child.TopLevel = false;
            child.Dock = System.Windows.Forms.DockStyle.Fill;//適應窗體大小
            child.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
            child.Parent = this.childPanel;
            this.childPanel.Controls.Add(child);
            child.Show();
        }

        private void btnLayout_Click(object sender, EventArgs e)
        {
            ShowChildForm(frmLayout);
        }

        private void btnRM2_Click(object sender, EventArgs e)
        {
            ShowChildForm(frmBufferInfo);
        }
    }
}
