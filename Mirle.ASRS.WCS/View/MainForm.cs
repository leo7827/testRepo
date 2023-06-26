#region Header
///   
///   ///	DESCRIPTION:
///	
///    History
///	***********************************************************************************************
///     Date            Editor      Version         Description
///	***********************************************************************************************
///    2022/11/01       力升      	V 1.0.0.0      初版
///    2022/11/18       力升      	V 1.0.0.1      開關門的邏輯優化
///    2022/12/20       力升      	V 1.0.0.2      平台動作優化
///    2023/01/07       力升      	V 1.0.0.3      滾出的時候不看有沒有這筆命令
///    2023/01/17       力升      	V 1.0.0.4      新增OPC  
///    2023/04/20       力升      	V 1.0.0.5      改寫 OPC的做法為 call web service , ini 通知由word 改成 bit
///    2023/05/03       力升      	V 1.0.0.6      新增外部可以呼叫電梯
#endregion Header


using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;
using Mirle.Grid.V2BYMA30;
using System.Collections.Generic;
using Mirle.LiteOn.V2BYMA30;
using Mirle.Def;
using Mirle.Def.V2BYMA30;
using Mirle.Structure.Info;
using Mirle.DB.Object;
using Mirle.DataBase;
using Mirle.WebAPI.Event.V2BYMA30;
using Unity;
using Mirle.Logger;
using Mirle.WebAPI.V2BYMA30.View;
using Mirle.ASRS.Close.Program;
using System.Threading;
using Mirle.Structure;
using Mirle.WebAPI.V2BYMA30.ReportInfo;
using Mirle.ASRS.Conveyor.V2BYMA30_10F.View;
using Mirle.DB.Object.Table;

namespace Mirle.ASRS.WCS.View
{
    public partial class MainForm : Form
    {
        
        public static clsCheckPathIsWork CheckPathIsWork = new clsCheckPathIsWork();
          
        public static clsElevatorCommand_UpDown_Proc ElevatorCommand_UpDown_Proc = new clsElevatorCommand_UpDown_Proc();
        public static clsElevator_OpenAndRoll_Proc Elevator_OpenAndRoll_Proc = new clsElevator_OpenAndRoll_Proc();

        public static cls8F_Proc Conveyor8F_Proc = new cls8F_Proc();      
        public static cls10F_Proc Conveyor10F_Proc = new cls10F_Proc();

        public static clsAlarm_Proc Alarm_Proc = new clsAlarm_Proc();
        public static clsAlarm_Proc_8F Alarm_Proc_8F = new clsAlarm_Proc_8F();
        public static clsAlarm_Proc_10F Alarm_Proc_10F = new clsAlarm_Proc_10F();
        public static clsCmdDestinationCheck_Proc CmdDestinationCheck_Proc = new clsCmdDestinationCheck_Proc();
         

        public static clsEMPTY_BIN_LOAD_REQUEST_Proc EMPTY_BIN_LOAD_REQUEST_Proc = new clsEMPTY_BIN_LOAD_REQUEST_Proc();

        public static clsPositionReport_Proc PositionReport_Proc = new clsPositionReport_Proc();
        public static clsPositionReport8F_Proc PositionReport8F_Proc = new clsPositionReport8F_Proc();
        public static clsPositionReport10F_Proc PositionReport10F_Proc = new clsPositionReport10F_Proc();

        private WebApiHost _webApiHost;
        private UnityContainer _unityContainer;
        private static System.Timers.Timer timRead = new System.Timers.Timer();
        private static System.Timers.Timer timRead_3F = new System.Timers.Timer();

        //private clsOPCStart OPCRun = new clsOPCStart();
        private clsOPCWebService OPCRun = new clsOPCWebService();

        //V 1.0.0.6
        public static clsPLCModeChange_Proc PLCModeChange_Proc = new clsPLCModeChange_Proc();
        public static clsPLCModeChange_Proc_10F PLCModeChange_Proc_10F = new clsPLCModeChange_Proc_10F();

        public MainForm()
        {
            InitializeComponent();

            timRead.Elapsed += new System.Timers.ElapsedEventHandler(timRead_Elapsed);
            timRead.Enabled = true; timRead.Interval = 500;

        
        }

        #region Event
        private void MainForm_Load(object sender, EventArgs e)
        {
            ChkAppIsAlreadyRunning();
            this.Text = this.Text + "  v " + ProductVersion;
            clInitSys.FunLoadIniSys();
            FunInit();
            FunEventInit();
            GridInit();

            clsWriLog.Log.FunWriTraceLog_CV("ASRS程式已開啟");
            //timRead.Enabled = true;
            timer1.Enabled = true;

        }

        private void FunEventInit()
        {
            //clsLiteOnStocker.GetStockerById(1).GetCraneById(1).OnStatusChanged += Stocker_OnStatusChanged_1;
            //clsLiteOnStocker.GetStockerById(2).GetCraneById(1).OnStatusChanged += Stocker_OnStatusChanged_2;
            //clsLiteOnStocker.GetStockerById(3).GetCraneById(1).OnStatusChanged += Stocker_OnStatusChanged_3;
            //clsLiteOnStocker.GetStockerById(4).GetCraneById(1).OnStatusChanged += Stocker_OnStatusChanged_4;
            //clsLiteOnCV.GetMainView_Object().GetMonitor().OnStkLabelClick += MainForm_OnStkLabelClick;

            //for (int i = 1; i <= Conveyor.V2BYMA30_3F.Signal.SignalMapper.BufferCount; i++)
            //{
                //clsLiteOnCV.GetConveyorController_3F().GetBuffer(i).OnStatusChanged += Buffer_OnStatusChanged;
                //clsLiteOnCV.GetConveyorController_3F().GetBuffer(i).OnPresenceChanged += Buffer_OnPresenceChanged;
                //clsLiteOnCV.GetConveyorController_3F().GetBuffer(i).OnAlarmBitChanged += MainForm_OnAlarmBitChanged;
                //clsLiteOnCV.GetConveyorController_3F().GetBuffer(i).OnAlarmBitChanged_2 += MainForm_OnAlarmBitChanged_2;
            //}

            //stockoutToStnCheck.OnLoadPortDataChanged += StockoutToStnCheck_OnLoadPortDataChanged;
        }

       
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmCloseProgram objCloseProgram;
            try
            {
                e.Cancel = true;

                objCloseProgram = new frmCloseProgram();

                if (objCloseProgram.ShowDialog() == DialogResult.OK)
                {
                    chkOnline.Checked = false;
                    SpinWait.SpinUntil(() => false, 1000);
                    clsWriLog.Log.FunWriTraceLog_CV("WCS程式已關閉！");
                    throw new Exception();
                }
            }
            catch
            {
                Environment.Exit(0);
            }
            finally
            {
                objCloseProgram = null;
            }
        }

        private MainTestForm mainTest;
        private void button1_Click(object sender, EventArgs e)
        {
            if (mainTest == null)
            {
                mainTest = new MainTestForm(clInitSys.WcsApi_Config);
                mainTest.TopMost = true;
                mainTest.FormClosed += new FormClosedEventHandler(funMainTest_FormClosed);
                mainTest.Show();
            }
            else
            {
                mainTest.BringToFront();
            }
        }

        private void funMainTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mainTest != null)
                mainTest = null;
        }


        
       
        //private StockerModeMaintainView stockerMode;
        //private void btnStockerModeMaintain_Click(object sender, EventArgs e)
        //{
        //    if (stockerMode == null)
        //    {
        //        stockerMode = new StockerModeMaintainView();
        //        stockerMode.TopMost = true;
        //        stockerMode.FormClosed += new FormClosedEventHandler(funStockerModeMaintain_FormClosed);
        //        stockerMode.Show();
        //    }
        //    else
        //    {
        //        stockerMode.BringToFront();
        //    }
        //}

        //private void funStockerModeMaintain_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    if (stockerMode != null)
        //        stockerMode = null;
        //}


        private void chkOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOnline.Checked)
                clsWriLog.Log.FunWriTraceLog_CV("WCS OnLine.");
            else
                clsWriLog.Log.FunWriTraceLog_CV("WCS OffLine.");

            //for (int i = 1; i <= 4; i++)
            //{
                //clsLiteOnCV.GetMainView_Object().GetMonitor().FunSetOnline(i, chkOnline.Checked);
            //}
        }

        
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl + L
            if (e.KeyCode == Keys.L && e.Modifiers == Keys.Control)
            {
                Def.clsTool.FunVisbleChange(ref button1);
                Def.clsTool.FunVisbleChange(ref btnCall10FCV);
                Def.clsTool.FunVisbleChange(ref chkOPC);
            }
        }

        private frmCmdMaintance cmdMaintance;
        private void btnCmdMaintain_Click(object sender, EventArgs e)
        {
            if (cmdMaintance == null)
            {
                cmdMaintance = new frmCmdMaintance();
                cmdMaintance.TopMost = true;
                cmdMaintance.FormClosed += new FormClosedEventHandler(funCmdMaintain_FormClosed);
                cmdMaintance.Show();
            }
            else
            {
                cmdMaintance.BringToFront();
            }
        }

        private void funCmdMaintain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cmdMaintance != null)
                cmdMaintance = null;
        }
         

        

        private Form[] stkcView = new Form[4];
        private void MainForm_OnStkLabelClick(object sender, Conveyor.V2BYMA30_10F.Events.StkLabelClickArgs e)
        {
            if (stkcView[e.StockerID - 1] == null)
                //stkcView[e.StockerID - 1] = clsLiteOnStocker.GetSTKCHostById(e.StockerID).GetMainView();
            stkcView[e.StockerID - 1].Show();
            stkcView[e.StockerID - 1].Activate();
        }
         
        #endregion Event
        #region Timer      

     



        private void timRead_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            timRead.Enabled = false;
            try
            {                
                if (clsDB_Proc.DBConn)
                {
                    FunCommandProc();
                    FunCheckWCS_CmdFinish();
                    FunMoveCmdToHis();
                    FunDeleteTask();
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timRead.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            try
            {
                lblDBConn_WCS.BackColor = clsDB_Proc.DBConn ? Color.Blue : Color.Red;
                lblDBConn_WMS.BackColor = clsDB_Proc.WMS_DBConn ? Color.Blue : Color.Red;
                lblTimer.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                timer1.Enabled = true;
            }
        }
        #endregion Timer
        


        private void FunCommandProc()
        {
            try
            {
                SubShowCmdtoGrid(ref Grid1);
               
               
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }


        private void FunMoveCmdToHis()
        {
            string sCmdSno_Ex = "";
            try
            {
                if (Grid1.RowCount > 0)
                {
                    for (int i = 0; i <= Grid1.RowCount - 1; i++)
                    {
                        string sCmdSno = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdSno.Index, i].Value);
                        sCmdSno_Ex = sCmdSno;
                        string sCmdMode = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdMode.Index, i].Value);
                        string sCurLoc = Convert.ToString(Grid1[ColumnDef.CMD_MST.CurLoc.Index, i].Value);
                        string sDestination = Convert.ToString(Grid1[ColumnDef.CMD_MST.Destination.Index, i].Value);
                        string sSource = Convert.ToString(Grid1[ColumnDef.CMD_MST.Source.Index, i].Value);
                        string sCmdSts = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdSts.Index, i].Value);
                        string sNewCmdSts = ((int)clsEnum.TaskCmdState.Finish).ToString();

                        if (sCmdSts == ((int)clsEnum.TaskCmdState.Complete).ToString())
                        {
                            if (clsTask.FunInsTaskHisAndDle(sCmdSno))
                            {
                                clsWriLog.Log.FunWriTraceLog_CV($"<Destination> {sDestination} <任務號> {sCmdSno} => 移至History！ ");
                            }

                            
                        }


                    }
                }
            }

            catch (Exception ex)
            {
                clsCmd_Mst.FunUpdateRemark(sCmdSno_Ex, ex.Message);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }

        }

        /// <summary>
        /// 遺留的命令, 超過一天就刪掉
        /// </summary>
        private void FunDeleteTask()
        {
            string sCmdSno_Ex = "";
            try
            {
                if (Grid1.RowCount > 0)
                {
                    for (int i = 0; i <= Grid1.RowCount - 1; i++)
                    {
                        string sCmdSno = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdSno.Index, i].Value);
                        sCmdSno_Ex = sCmdSno;
                        string sCmdMode = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdMode.Index, i].Value);
                        string sCurLoc = Convert.ToString(Grid1[ColumnDef.CMD_MST.CurLoc.Index, i].Value);
                        string sDestination = Convert.ToString(Grid1[ColumnDef.CMD_MST.Destination.Index, i].Value);
                        string sSource = Convert.ToString(Grid1[ColumnDef.CMD_MST.Source.Index, i].Value);
                        string sCmdSts = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdSts.Index, i].Value);
                        string sCrtDate = Convert.ToString(Grid1[ColumnDef.CMD_MST.CrtDate.Index, i].Value);

                        DateTime dNow = DateTime.Now;
                        DateTime dCrtDate = Convert.ToDateTime(sCrtDate);

                        TimeSpan tDiff = dNow.Subtract(dCrtDate);
                        if (tDiff.Days >= 1)
                        {
                            if (clsTask.FunDeleteTask(sCmdSno) )
                            {
                                clsWriLog.Log.FunWriTraceLog_CV($"<Destination> {sDestination} <任務號> {sCmdSno} => 閒置超過一天 , 已刪除！ ");
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                clsCmd_Mst.FunUpdateRemark(sCmdSno_Ex, ex.Message);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        /// <summary>
        /// 判斷WCS命令是否完成
        /// </summary>
        private void FunCheckWCS_CmdFinish()
        {
            string sCmdSno_Ex = "";
            try
            {
                if (Grid1.RowCount > 0)
                {
                    for (int i = 0; i <= Grid1.RowCount - 1; i++)
                    {
                        string sCmdSno = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdSno.Index, i].Value);
                        sCmdSno_Ex = sCmdSno;
                        string sCmdMode = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdMode.Index, i].Value);
                        string sCurLoc = Convert.ToString(Grid1[ColumnDef.CMD_MST.CurLoc.Index, i].Value);
                        string sDestination = Convert.ToString(Grid1[ColumnDef.CMD_MST.Destination.Index, i].Value);
                        string sSource = Convert.ToString(Grid1[ColumnDef.CMD_MST.Source.Index, i].Value);
                        string sCmdSts = Convert.ToString(Grid1[ColumnDef.CMD_MST.CmdSts.Index, i].Value);

                        if (sDestination == "LO1-08")
                        {
                            sDestination = "LO1-06";
                        }

                        if (sCurLoc == sDestination && !string.IsNullOrWhiteSpace(sDestination) && sCmdSts == ((int)clsEnum.TaskCmdState.Moving).ToString() )
                        {
                            string sRemark = "", strErrMsg = "";
                            switch (sCmdMode)
                            {
                                case clsConstValue.CmdMode.StockOut:
                                    sRemark = "出庫命令已完成";
                                    break;
                                case clsConstValue.CmdMode.StockIn:
                                    sRemark = "入庫命令已完成";
                                    break;
                                default:
                                    sRemark = "盤點命令已完成";
                                    break;

                            }
                            sCmdSts = ((int)clsEnum.TaskCmdState.Complete).ToString();

                            if (clsTask.FunUpdateTaskCmd(sCmdSno, sCmdSts, sCurLoc, sRemark, ref strErrMsg))
                            {
                                clsWriLog.Log.FunWriTraceLog_CV($"<Destination> {sDestination} <任務號> {sCmdSno} => 搬運結束！ ");
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCmd_Mst.FunUpdateRemark(sCmdSno_Ex, ex.Message);
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private void FunInit()
        {
            //clsDB_Proc.Initial(Application.StartupPath + "\\Sqlite\\", "LCSCODE.db");
            var archive = new AutoArchive();
            archive.Start();
            //clsDB_Proc.Initial(clInitSys.DbConfig); 
            clsDB_Proc.Initial(Application.StartupPath + "\\Sqlite\\", "LCSCODE.db");

            clsLiteOnCV.FunInitalCVController(clInitSys.CV_Config_8F, 1);
            clsLiteOnCV.FunInitalCVController(clInitSys.CV_Config_10F, 2);
            clsLiteOnCV.FunInitalCVController(clInitSys.CV_Config_Ele, 3);

            clsWmsApi.FunInit(clInitSys.WcsApi_Config);

            OPCRun.Initialize();
            PositionReport_Proc.subStart();
            PositionReport8F_Proc.subStart();
            PositionReport10F_Proc.subStart();

            EMPTY_BIN_LOAD_REQUEST_Proc.subStart();

            Alarm_Proc.subStart();
            Alarm_Proc_8F.subStart();
            Alarm_Proc_10F.subStart();
            Elevator_OpenAndRoll_Proc.subStart();
            ElevatorCommand_UpDown_Proc.subStart();
            Conveyor8F_Proc.subStart();
            Conveyor10F_Proc.subStart();

            PLCModeChange_Proc.subStart();
            PLCModeChange_Proc_10F.subStart();
            //CmdDestinationCheck_Proc.subStart();

            _unityContainer = new UnityContainer();
            _unityContainer.RegisterInstance(new WCSController());
            _webApiHost = new WebApiHost(new Startup(_unityContainer), clInitSys.LocalApi_Config.IP);
        
            ChangeSubForm(clsLiteOnCV.GetMainView(2));
            //FunInitStockerStsForm();
        }

        #region Grid顯示
        private void GridInit()
        {
            Gird.clInitSys.GridSysInit(ref Grid1);
            ColumnDef.CMD_MST.GridSetLocRange(ref Grid1);
        }

        delegate void degShowCmdtoGrid(ref DataGridView oGrid);
        private void SubShowCmdtoGrid(ref DataGridView oGrid)
        {
            degShowCmdtoGrid obj;
            string strSql = string.Empty;
            string strEM = string.Empty;
            DataTable dtTmp = new DataTable();
            try
            {
                if (InvokeRequired)
                {
                    obj = new degShowCmdtoGrid(SubShowCmdtoGrid);
                    Invoke(obj, oGrid);
                }
                else
                {
                    oGrid.SuspendLayout();
                    oGrid.Rows.Clear();
                    int iRet = clsCmd_Mst.FunGetTaskInfo_Grid(ref dtTmp);
                    //int iRet = DBResult.Failed;
                    if (iRet == DBResult.Success)
                    {
                        for (int i = 0; i < dtTmp.Rows.Count; i++)
                        {
                            oGrid.Rows.Add();
                            oGrid.Rows[oGrid.RowCount - 1].HeaderCell.Value = Convert.ToString(oGrid.RowCount);
                            oGrid[ColumnDef.CMD_MST.CmdSno.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CmdSno"]);
                            oGrid[ColumnDef.CMD_MST.CmdSts.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CmdSts"]);
                            oGrid[ColumnDef.CMD_MST.prty.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["prty"]);
                            oGrid[ColumnDef.CMD_MST.Source.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["sFrom"]);
                            oGrid[ColumnDef.CMD_MST.Destination.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["sTo"]);
                            oGrid[ColumnDef.CMD_MST.CmdMode.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CmdMode"]);
                            oGrid[ColumnDef.CMD_MST.CrtDate.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CrtDate"]);
                            oGrid[ColumnDef.CMD_MST.ExpDate.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["ExpDate"]);
                            oGrid[ColumnDef.CMD_MST.EndDate.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["EndDate"]);
                            oGrid[ColumnDef.CMD_MST.CurLoc.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CurLoc"]);
                            oGrid[ColumnDef.CMD_MST.Remark.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["Remark"]);
                            oGrid[ColumnDef.CMD_MST.CarrierType.Index, oGrid.Rows.Count - 1].Value = Convert.ToString(dtTmp.Rows[i]["CarrierType"]);
                        }
                    }
                    oGrid.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
            finally
            {
                dtTmp = null;
            }
        }
        #endregion Grid顯示

        /// <summary>
        /// 檢查程式是否重複開啟
        /// </summary>
        private void ChkAppIsAlreadyRunning()
        {
            try
            {
                string aFormName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                string aProcName = System.IO.Path.GetFileNameWithoutExtension(aFormName);
                if (System.Diagnostics.Process.GetProcessesByName(aProcName).Length > 1)
                {
                    MessageBox.Show("程式已開啟", "Communication System", MessageBoxButtons.OK);
                    //Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                Environment.Exit(0);
            }
        }

        private void ChangeSubForm(Form subForm)
        {
            try
            {
                var children = spcMainView.Panel1.Controls;
                foreach (Control c in children)
                {
                    if (c is Form)
                    {
                        var thisChild = c as Form;
                        //thisChild.Hide();
                        spcMainView.Panel1.Controls.Remove(thisChild);
                        thisChild.Width = 0;
                    }
                }

                if (subForm != null)
                {
                    subForm.TopLevel = false;
                    subForm.Dock = DockStyle.Fill;//適應窗體大小
                    subForm.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
                    subForm.Parent = spcMainView.Panel1;
                    spcMainView.Panel1.Controls.Add(subForm);
                    subForm.Show();
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private void FunInitStockerStsForm()
        {
            //var subForm = clsLiteOnStocker.GetStockerStsView();
            //subForm.TopLevel = false;
            //subForm.Dock = DockStyle.Fill;//適應窗體大小
            //subForm.FormBorderStyle = FormBorderStyle.None;//隱藏右上角的按鈕
            //subForm.Parent = tlpMainSts;
            //tlpMainSts.Controls.Add(subForm, 2, 0);
            //subForm.Show();
        }

     

        private void btnCall8FCV_Click(object sender, EventArgs e)
        {
            ChangeSubForm(clsLiteOnCV.GetMainView(1));
        }

        private void btnCall10FCV_Click(object sender, EventArgs e)
        {
            ChangeSubForm(clsLiteOnCV.GetMainView(2));
        }

        private void btnCallEleCV_Click(object sender, EventArgs e)
        {
            ChangeSubForm(clsLiteOnCV.GetMainView(3));
        }

        private void chkOPC_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkOPC.Checked)
            {
                OPCRun.Stop();
            }
            else
            {
                OPCRun.Start();
            }
        }
    }
}
