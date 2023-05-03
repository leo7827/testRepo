using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Mirle.DataBase;
using Mirle.DB.Object;
using System.Windows.Forms;
using Mirle.Grid.V2BYMA30;
using Mirle.DB.Object.Table;

namespace Mirle.ASRS.WCS.View
{
    public partial class frmCmdMaintance : Form
    {
        public frmCmdMaintance()
        {
            InitializeComponent();
        }

        private void frmCmdMaintance_Load(object sender, EventArgs e)
        {
            GridInit();                 //畫面初始化-Grid
            btnQuery.PerformClick();
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

        private void btnQuery_Click(object sender, EventArgs e)
        {
            btnQuery.Enabled = false;
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
            finally
            {
                btnQuery.Enabled = true;
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            string strEM = string.Empty;
            btnModify.Enabled = false;
            try
            {
                if (Grid1.SelectedRows.Count > 0)
                {
                    string sCmdSno = Grid1.SelectedRows[0].Cells[ColumnDef.CMD_MST.CmdSno.Index].Value.ToString();
                    DialogResult DlgResult = MessageBox.Show("任務號：" + sCmdSno + "\n確定強制完成？", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DlgResult == DialogResult.Yes)
                    {
                        //if (clsManual.FunManualCommandComplete(sCmdSno, ref strEM))
                        if (clsTask.FunUpdateTaskCmd(sCmdSno, "7", " ", " ", ref strEM))
                        {
                            clsWriLog.Log.FunWriTraceLog_CV($"<任務號> {sCmdSno} => 手動完成命令成功！");
                        }
                        else
                        {
                            clsWriLog.Log.FunWriTraceLog_CV($"NG: <任務號> {sCmdSno} => 手動完成命令失敗 [{strEM}]");
                        }
                    }
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
                btnQuery.PerformClick();
                btnModify.Enabled = true;
            }
        }

        private void btnRepeatCmd_Click(object sender, EventArgs e)
        {
            string strEM = string.Empty;
            btnRepeatCmd.Enabled = false;
            try
            {
                if (Grid1.SelectedRows.Count > 0)
                {
                    string sCmdSno = Grid1.SelectedRows[0].Cells[ColumnDef.CMD_MST.CmdSno.Index].Value.ToString();
                    DialogResult DlgResult = MessageBox.Show("任務號：" + sCmdSno + "\n確定重新執行命令？", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DlgResult == DialogResult.Yes)
                    {
                        if (clsManual.FunManualRepeatCmd(sCmdSno, ref strEM))
                        {
                            clsWriLog.Log.FunWriTraceLog_CV($"<任務號> {sCmdSno} => 手動重新執行命令成功！");
                        }
                        else
                        {
                            clsWriLog.Log.FunWriTraceLog_CV($"NG: <任務號> {sCmdSno} => 手動重新執行命令失敗 [{strEM}]");
                        }
                    }
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
                btnQuery.PerformClick();
                btnRepeatCmd.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string strEM = string.Empty;
            btnCancel.Enabled = false;
            try
            {
                if (Grid1.SelectedRows.Count > 0)
                {
                    string sCmdSno = Grid1.SelectedRows[0].Cells[ColumnDef.CMD_MST.CmdSno.Index].Value.ToString();
                    DialogResult DlgResult = MessageBox.Show("任務號：" + sCmdSno + "\n確定強制結束？", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DlgResult == DialogResult.Yes)
                    {
                        //if (clsManual.FunManualCommandCancel(sCmdSno, ref strEM))
                        //{
                        //    clsWriLog.Log.FunWriTraceLog_CV($"<任務號> {sCmdSno} => 手動結束命令成功！");
                        //}
                        //else
                        //{
                        //    clsWriLog.Log.FunWriTraceLog_CV($"NG: <任務號> {sCmdSno} => 手動結束命令失敗 [{strEM}]");
                        //}
                    }
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
                btnQuery.PerformClick();
                btnCancel.Enabled = true;
            }
        }
    }
}
