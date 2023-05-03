using System;
using System.Windows.Forms;
using Mirle.MPLC;

namespace Mirle.Stocker.R46YP320.View
{
    public partial class MonitorMainForm : Form
    {
        private IMPLCProvider _mplc;
        private readonly MPLCViewController _mplcViewController;
        private SignalMapper4_11 _spec;
        private CSOTStocker _stocker;

        private MonitorCrane frmRM1;
        private MonitorCrane frmRM2;
        private MonitorPort frmPort;

        public MonitorMainForm(IMPLCProvider mplc, MPLCViewController mplcViewController)
        {
            InitializeComponent();

            _mplc = mplc;
            _mplcViewController = mplcViewController;
            _spec = new SignalMapper4_11(_mplc);
            _stocker = new CSOTStocker(_spec, LCSEnums.ControlMode.Dual);

            this.frmRM1 = new MonitorCrane(_stocker.GetCraneById(1) as Crane, _mplcViewController);
            this.frmRM2 = new MonitorCrane(_stocker.GetCraneById(2) as Crane, _mplcViewController);
            this.frmPort = new MonitorPort(_stocker, _mplcViewController);
        }

        private void MonitorMainForm_Load(object sender, EventArgs e)
        {
            ShowChildForm(frmRM1);
            this.RefreshTimer.Enabled = true;
        }

        private void btnRM1_Click(object sender, EventArgs e)
        {
            ShowChildForm(frmRM1);
        }

        private void btnRM2_Click(object sender, EventArgs e)
        {
            ShowChildForm(frmRM2);
        }

        private void btnPort_Click(object sender, EventArgs e)
        {
            ShowChildForm(frmPort);
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

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            lblSharedArea.Text = _spec.Stocker.ShareArea_StartBay.GetValue() + ">" + _spec.Stocker.ShareArea_EndBay.GetValue();
            lblHandoffArea.Text = _spec.Stocker.HandOff_StartBay.GetValue() + ">" + _spec.Stocker.HandOff_EndBay.GetValue();
        }
    }
}
