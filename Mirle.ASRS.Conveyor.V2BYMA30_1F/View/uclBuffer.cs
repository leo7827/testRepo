using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.View
{
    public partial class uclBuffer : UserControl
    {
        #region 變數
        private enuCmdMode objCmdMode = enuCmdMode.None;
        //private enuStnMode objStnMode = enuStnMode.None;
        private enuReady objReady = enuReady.NoReady;
        private bool bolDone = false;
        private bool bolPosition = false;
        private int intReadNotice = 0;
        private int intInitialNotice = 0;
        private int objPathNotice = 0;
        private bool bolError = false;
        private bool bolAuto = false;
        private bool bolLoad = false;

        #endregion 變數

        #region 事件
        /// <summary>
        /// 發生於按下uclBuffer控制項時
        /// </summary>
        public static event EventHandler uclBufferClick;
        #endregion 事件

        #region 委派
        private delegate void delUpdate(Label label, string Value);
        private delegate void delUpdateOnlyColor(Label label, Color color);
        private delegate void delUpdateWithColor(Label label, string Value, Color color);
        #endregion 委派

        #region 列舉
        /// <summary>
        /// 命令模式列舉
        /// </summary>
        public enum enuCmdMode
        {
            None = 0,  //初始值
            Inbound = 1, //入庫
            Outbound = 2, //出庫
            CycleCount = 3, //盤點
            S2S = 4, //站對站
            R2R = 5, //庫對庫
        }

        /// <summary>
        /// Ready列舉
        /// </summary>
        public enum enuReady
        {
            NoReady = 0,
            InReady = 1,   // 入庫Ready
            OutReady = 2   //出庫Ready
        }

        /// <summary>
        /// 讀取通知列舉
        /// </summary>
        public enum enuReadNotice
        {
            None = 0,    //Initial
            Read = 1,
        }
        #endregion 列舉

        #region 屬性

        #region 控制項文字
        /// <summary>
        /// Buffer編號
        /// </summary>
        [Category("自訂屬性"), Description("Buffer編號")]
        public string BufferName
        {
            get
            {
                return lblBufferName.Text ;
            }
            set
            {
                funUpdate(lblBufferName, value);
            }
        }

        /// <summary>
        /// 命令序號
        /// </summary>
        [Category("自訂屬性"), Description("命令序號")]
        public string CmdSno
        {
            get
            {
                return lblCmdSno.Text ;
            }
            set
            {
                funUpdate(lblCmdSno, value);
            }
        }

        /// <summary>
        /// 命令模式
        /// </summary>
        [Category("自訂屬性"), Description("命令模式")]
        public enuCmdMode CmdMode
        {
            get
            {   return objCmdMode;  }
            set
            {
                objCmdMode = value;
                funUpdate(lblCmdMode , ((int)objCmdMode).ToString());
            }
        }

        /// <summary>
        /// Ready
        /// </summary>
        [Category("自訂屬性"), Description("Ready")]
        public enuReady Ready
        {
            get
            {
                return objReady;
            }
            set
            {
                objReady = value;

                if (objReady != enuReady.NoReady)
                {
                    funUpdate(lblReady, ((int)objReady).ToString(), Color.LightGreen);
                }
                else
                {
                    funUpdate(lblReady, ((int)objReady).ToString(), Color.White);
                }
            }
        }

        /// <summary>
        /// 讀取通知
        /// </summary>
        [Category("自訂屬性"), Description("讀取通知")]
        public int ReadNotice
        {
            get
            {
                return intReadNotice;
            }
            set
            {
                intReadNotice = value;
                if (intReadNotice > 0)
                { funUpdate(lblReadNotice, intReadNotice.ToString(),Color .LightGreen ); }
                else
                { funUpdate(lblReadNotice, intReadNotice.ToString(), Color.White ); }
            }
        }

        /// <summary>
        /// 路徑
        /// </summary>
        [Category("自訂屬性"), Description("路徑通知")]
        public int PathNotice
        {
            get
            {
                return objPathNotice;
            }
            set
            {
                objPathNotice = value;
                funUpdate(lblPathNotice , objPathNotice.ToString());
            }
        }

        /// <summary>
        /// 初始通知
        /// </summary>
        [Category("自訂屬性"), Description("初始通知")]
        public int InitialNotice
        {
            get
            {
                return intInitialNotice;
            }
            set
            {
                intInitialNotice = value;
                if (intInitialNotice == 1)
                { funUpdate(lblInitialNotice, intInitialNotice.ToString(), Color.LightGreen); }
                else
                { funUpdate(lblInitialNotice, intInitialNotice.ToString(), Color.White ); }
            }
        }

        /// <summary>
        /// Error
        /// </summary>
        [Category("自訂屬性"), Description("Error")]
        public bool Error
        {
            get
            {
                return bolError;
            }
            set
            {
                bolError = value;
                if (bolError ==true   )
                {
                    funUpdate(lblError, "X", Color.Red );
                }
                else
                {
                    funUpdate(lblError, string.Empty, Color.White);
                }
            }
        }

        /// <summary>
        /// 自動
        /// </summary>
        [Category("自訂屬性"), Description("自動")]
        public bool Auto
        {
            get
            {
                return bolAuto ;
            }
            set
            {
                bolAuto = value;
                if (bolAuto == false )
                {
                    funUpdate(lblAuto , "M", Color.Red );
                }
                else
                {
                    funUpdate(lblAuto, "A", Color.LightGreen);
                }
            }
        }

        /// <summary>
        /// 荷有
        /// </summary>
        [Category("自訂屬性"), Description("荷有")]
        public bool bLoad
        {
            get
            {
                return bolLoad ;
            }
            set
            {
                bolLoad = value;
                if (bolLoad == false )
                {
                    funUpdate(lblLoad , string.Empty  , Color.White );
                    funUpdate(lblBufferName, Color.Green);
                    lblBufferName.ForeColor = Color.White;
                }
                else
                {
                    funUpdate(lblLoad, "V", Color.Orange);
                    funUpdate(lblBufferName, Color.Orange); 
                    lblBufferName.ForeColor = Color.Black;
                }
            }
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        [Category("自訂屬性"), Description("作业完了")]
        public bool Done
        {
            get
            {
                return bolDone ;
            }
            set
            {
                bolDone = value;
                if (bolDone == true)
                { funUpdate(lblDone, "1",Color .LightGreen ); }
                else
                { funUpdate(lblDone, "0", Color.White ); }
            }
        }

        /// <summary>
        /// 定位
        /// </summary>
        [Category("自訂屬性"), Description("Position")]
        public bool Position
        {
            get
            {
                return bolPosition;
            }
            set
            {
                bolPosition = value;
                if (bolPosition == true)
                { funUpdate(lblPosition, "1", Color.LightGreen); }
                else
                { funUpdate(lblPosition, "0", Color.White); }
            }
        }
        #endregion 控制項文字

        #region 控制項背景顏色
        /// <summary>
        /// Buffer編號背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("Buffer編號背景顏色")]
        public Color BufferNameColor
        {
            get
            {
                return lblBufferName.BackColor;
            }
            set
            {
                funUpdate(lblBufferName, value);
            }
        }

        public Color BufferNameFrontColor
        {
            get
            {
                return lblBufferName.ForeColor;
            }
            set
            {
                funUpdateFrontColor(lblBufferName, value);
            }
        }

        /// <summary>
        /// 命令序號背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("序號背景顏色")]
        public Color CmdSnoColor
        {
            get
            {
                return lblCmdSno.BackColor;
            }
            set
            {
                funUpdate(lblCmdSno, value);
            }
        }

        /// <summary>
        /// 命令模式背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("命令模式背景顏色")]
        public Color CmdModeColor
        {
            get
            {
                return lblCmdMode.BackColor;
            }
            set
            {
                funUpdate(lblCmdMode, value);
            }
        }

        /// <summary>
        /// Ready背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("Ready背景顏色")]
        public Color ReadyColor
        {
            get
            {
                return lblReady.BackColor;
            }
            set
            {
                funUpdate(lblReady, value);
            }
        }

        /// <summary>
        /// 讀取通知背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("讀取通知背景顏色")]
        public Color ReadNoticeColor
        {
            get
            {
                return lblReadNotice.BackColor;
            }
            set
            {
                funUpdate(lblReadNotice, value);
            }
        }

        /// <summary>
        /// 路徑背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("路徑背景顏色")]
        public Color PathNoticeColor
        {
            get
            {
                return lblPathNotice.BackColor;
            }
            set
            {
                funUpdate(lblPathNotice, value);
            }
        }

        /// <summary>
        /// 站口模式切換背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("初始通知背景顏色")]
        public Color InitialNoticeColor
        {
            get
            {
                return lblInitialNotice.BackColor;
            }
            set
            {
                funUpdate(lblInitialNotice, value);
            }
        }

        /// <summary>
        /// Error背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("Error背景顏色")]
        public Color ErrorColor
        {
            get
            {
                return lblError.BackColor;
            }
            set
            {
                funUpdate(lblError, value);
            }
        }

        /// <summary>
        /// 自動背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("自動背景顏色")]
        public Color AutoColor
        {
            get
            {
                return lblAuto.BackColor;
            }
            set
            {
                funUpdate(lblAuto, value);
            }
        }

        /// <summary>
        /// 荷有背景顏色
        /// </summary>
        [Category("自訂屬性"), Description("荷有背景顏色")]
        public Color LoadColor
        {
            get
            {
                return lblLoad.BackColor;
            }
            set
            {
                funUpdate(lblLoad, value);
            }
        }
        #endregion 控制項背景顏色

        #endregion 屬性

        #region 建構函式
        /// <summary>
        /// 初始化 Mirle.WinPLCCommu.uclBuffer 類別的新執行個體
        /// </summary>
        public uclBuffer()
        {
            InitializeComponent();

            funInitToolTip();
            funInit();
        }
        #endregion 建構函式

        #region 事件
        /// <summary>
        /// 表示 uclBuffer 觸發 Click 事件處理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uclBuffer_Click(object sender, EventArgs e)
        {
            if(uclBufferClick != null)
                uclBufferClick(this, e);
        }
        #endregion 事件

        #region 私用方法
        /// <summary>
        /// 初始化ToolTip
        /// </summary>
        private void funInitToolTip()
        {
            string strBufferName = "輸送機編號";
            string strCmdSno = "任務號";
            string strCmdMode = "模式:\n1 -> 入庫\n2 -> 出庫\n3 -> 盤點\n4 -> 站對站\n5 -> 庫對庫";
            string strReady = "Ready訊號:\n1 -> 入庫Ready\n2 -> 出庫Ready";
            string strReadNotice = "讀取通知:\n1 -> CV通知WCS去收條碼資料";
            string strPathNotice = "路徑通知:\n由WCS告知CV貨物該往哪走";
            string strInitialNotice = "初始通知:\n1 -> CV通知WCS輸送機初始已完成";
            string strError = "報錯顯示:\nX -> 輸送機異常中";
            string strAuto = "手自動狀態:\nA -> 自動模式\nM -> 手動模式";
            string strLoad = "載荷狀態:\nV -> 此位置有物";
            string strDone = "作業完了:\n1 -> User按了「完成」鈕";
            string strPortModeChangeable = "PortModeChangeable: \n1 -> ON\n0 -> OFF";

            ToolTip objToolTip = new ToolTip();
            objToolTip.AutoPopDelay = 5000;
            objToolTip.InitialDelay = 100;
            objToolTip.ReshowDelay = 100;
            objToolTip.UseAnimation = false;
            objToolTip.UseFading = false;
            objToolTip.ShowAlways = true;

            objToolTip.SetToolTip(lblBufferName,strBufferName);
            objToolTip.SetToolTip(lblCmdSno,strCmdSno);
            objToolTip.SetToolTip(lblCmdMode,strCmdMode);
            objToolTip.SetToolTip(lblReady,strReady);
            objToolTip.SetToolTip(lblReadNotice,strReadNotice);
            objToolTip.SetToolTip(lblPathNotice,strPathNotice);
            objToolTip.SetToolTip(lblInitialNotice,strInitialNotice);
            objToolTip.SetToolTip(lblError,strError);
            objToolTip.SetToolTip(lblDone,strDone );
            objToolTip.SetToolTip(lblAuto,strAuto);
            objToolTip.SetToolTip(lblLoad,strLoad);
            objToolTip.SetToolTip(lblPosition, strPortModeChangeable);
        }

        /// <summary>
        /// 初始化物件
        /// </summary>
        private void funInit()
        {
            lblBufferName.BackColor = Color.Black ;
            lblCmdSno.BackColor = Color.White;
            lblCmdMode.BackColor = Color.White;
            lblReady.BackColor = Color.White  ;
            lblReadNotice.BackColor = Color.White;
            lblPathNotice.BackColor = Color.White;
            lblInitialNotice.BackColor = Color.White;
            lblError.BackColor = Color.White  ;
            lblPosition.BackColor = Color.White ;
            lblDone.BackColor = Color.White ;
            lblAuto.BackColor = Color.White ;
            lblLoad.BackColor = Color.White;
            
            lblBufferName.Text = "BufName";
            lblCmdSno.Text =  string.Empty;

            lblCmdMode.Text = lblReadNotice.Text = lblReady.Text = lblPosition.Text = lblDone.Text = lblAuto.Text = lblPathNotice.Text = "0";
            lblInitialNotice.Text = lblPathNotice.Text = lblError.Text = lblLoad .Text  = "0";

            tlpBuffer.Size = new Size(this.Size.Width - 6, this.Size.Height - 6);
        }

        #region funUpdate
        private void funUpdate(Label label, string Value)
        {
            if(this.InvokeRequired)
            {
                delUpdate Update = new delUpdate(funUpdate);
                this.Invoke(Update, label, Value);
            }
            else
                label.Text = Value;
        }
        private void funUpdate(Label label, string Value, Color color)
        {
            if(this.InvokeRequired)
            {
                delUpdateWithColor Update = new delUpdateWithColor(funUpdate);
                this.Invoke(Update, label, Value, color);
            }
            else
            {
                label.Text = Value;
                label.BackColor = color;
            }
        }
        private void funUpdate(Label label, Color color)
        {
            if(this.InvokeRequired)
            {
                delUpdateOnlyColor Update = new delUpdateOnlyColor(funUpdate);
                this.Invoke(Update, label, color);
            }
            else
                label.BackColor = color;
        }
        private void funUpdateFrontColor(Label label, Color color)
        {
            if (this.InvokeRequired)
            {
                delUpdateOnlyColor Update = new delUpdateOnlyColor(funUpdateFrontColor);
                this.Invoke(Update, label, color);
            }
            else
                label.ForeColor = color;
        }
        #endregion funUpdate

        #endregion 私用方法

        #region 公用方法
        /// <summary>
        /// 當PLC讀取失敗或斷線時使用
        /// </summary>
        public void funReadPLCError()
        {
            CmdSno = string.Empty ;
            CmdMode = enuCmdMode.None;
            Ready = enuReady.NoReady;
            Done = false;
            InitialNotice = 0;
            PathNotice = 0;
            Position = false;
            Auto = false;
            ReadNotice =0  ;
            Error = true;
            bLoad = false;
        }
        #endregion 公用方法



 
    }
}
