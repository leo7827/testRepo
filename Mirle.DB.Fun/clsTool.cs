using System;
using Mirle.Def;
using Mirle.LiteOn.V2BYMA30;
using Mirle.Structure;
using Mirle.DataBase;
using System.Data;

namespace Mirle.DB.Fun
{
    public class clsTool
    {
        public CmdMstInfo GetCommand(DataTable dtTmp)
        {
            CmdMstInfo cmd = new CmdMstInfo
            {
                backupPortId = Convert.ToString(dtTmp.Rows[0]["backupPortId"]),
                BatchID = Convert.ToString(dtTmp.Rows[0]["BatchID"]),
                BoxID = Convert.ToString(dtTmp.Rows[0]["BoxId"]),
                CmdMode = Convert.ToString(dtTmp.Rows[0]["CmdMode"]),
                CmdSno = Convert.ToString(dtTmp.Rows[0]["CmdSno"]),
                CmdSts = Convert.ToString(dtTmp.Rows[0]["CmdSts"]),
                CrtDate = Convert.ToString(dtTmp.Rows[0]["CrtDate"]),
                CurDeviceID = Convert.ToString(dtTmp.Rows[0]["CurDeviceID"]),
                CurLoc = Convert.ToString(dtTmp.Rows[0]["CurLoc"]),
                EndDate = Convert.ToString(dtTmp.Rows[0]["EndDate"]),
                EquNo = Convert.ToString(dtTmp.Rows[0]["EquNO"]),
                ExpDate = Convert.ToString(dtTmp.Rows[0]["ExpDate"]),
                IoType = Convert.ToString(dtTmp.Rows[0]["Iotype"]),
                JobID = Convert.ToString(dtTmp.Rows[0]["JobID"]),
                Loc = Convert.ToString(dtTmp.Rows[0]["Loc"]),
                NeedShelfToShelf = Convert.ToString(dtTmp.Rows[0]["NeedShelfToShelf"]),
                NewLoc = Convert.ToString(dtTmp.Rows[0]["NewLoc"]),
                Prt = Convert.ToString(dtTmp.Rows[0]["PRT"]),
                Remark = Convert.ToString(dtTmp.Rows[0]["Remark"]),
                StnNo = Convert.ToString(dtTmp.Rows[0]["StnNo"]),
                Userid = Convert.ToString(dtTmp.Rows[0]["UserID"]),
                ZoneID = Convert.ToString(dtTmp.Rows[0]["ZoneID"]),
                ticketId = Convert.ToString(dtTmp.Rows[0]["ticketId"])
            };

            return cmd;
        }

        public TaskInfo GetCommandbyTask(DataTable dtTmp)
        {
            TaskInfo cmd = new TaskInfo
            {
                CmdSno = Convert.ToString(dtTmp.Rows[0]["CmdSno"]),
                CmdSts = Convert.ToString(dtTmp.Rows[0]["CmdSts"]),
                Prty = Convert.ToString(dtTmp.Rows[0]["Prty"]),
                Source = Convert.ToString(dtTmp.Rows[0]["sFrom"]),
                Destination = Convert.ToString(dtTmp.Rows[0]["sTo"]),
                CmdMode = Convert.ToString(dtTmp.Rows[0]["CmdMode"]),
                CrtDate = Convert.ToString(dtTmp.Rows[0]["CrtDate"]),
                ExpDate = Convert.ToString(dtTmp.Rows[0]["ExpDate"]),
                EndDate = Convert.ToString(dtTmp.Rows[0]["EndDate"]),
                CurLoc = Convert.ToString(dtTmp.Rows[0]["CurLoc"]),
                Remark = Convert.ToString(dtTmp.Rows[0]["Remark"]),
                CarrierType = Convert.ToString(dtTmp.Rows[0]["CarrierType"]),
            };

            return cmd;
        }
     
    }
}
