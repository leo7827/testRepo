using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mirle.Gird;

namespace Mirle.Grid.V2BYMA30
{
    public class ColumnDef
    {
        public class Teach
        {
            public static readonly ColumnInfo DeviceID = new ColumnInfo { Index = 0, Name = "DeviceID", Width = 60 };
            public static readonly ColumnInfo Loc = new ColumnInfo { Index = 1, Name = "Loc", Width = 68 };
            public static readonly ColumnInfo LocSts = new ColumnInfo { Index = 2, Name = "LocSts", Width = 60 };
            public static readonly ColumnInfo BoxId = new ColumnInfo { Index = 3, Name = "CstID", Width = 100 };

            public static void GridSetLocRange(ref DataGridView oGrid)
            {
                oGrid.ColumnCount = 4;
                oGrid.RowCount = 0;
                clInitSys.SetGridColumnInit(DeviceID, ref oGrid);
                clInitSys.SetGridColumnInit(Loc, ref oGrid);
                clInitSys.SetGridColumnInit(BoxId, ref oGrid);
                clInitSys.SetGridColumnInit(LocSts, ref oGrid);
            }
        }

        public class CMD_MST
        {
            public static readonly ColumnInfo CmdSno = new ColumnInfo { Index = 0, Name = "任務號", Width = 68 };
            public static readonly ColumnInfo CmdSts = new ColumnInfo { Index = 1, Name = "狀態", Width = 100 };
            public static readonly ColumnInfo prty = new ColumnInfo { Index = 2, Name = "優先權", Width = 50 };
            public static readonly ColumnInfo Source = new ColumnInfo { Index = 3, Name = "來源", Width = 100 };
            public static readonly ColumnInfo Destination = new ColumnInfo { Index = 4, Name = "目的", Width = 100 };
            public static readonly ColumnInfo CmdMode = new ColumnInfo { Index = 5, Name = "模式", Width = 60 };
            public static readonly ColumnInfo CrtDate = new ColumnInfo { Index = 6, Name = "創立時間", Width = 200 };
            public static readonly ColumnInfo ExpDate = new ColumnInfo { Index = 7, Name = "更新時間", Width = 200 };
            public static readonly ColumnInfo EndDate = new ColumnInfo { Index = 8, Name = "完成時間", Width = 250 };
            public static readonly ColumnInfo CurLoc = new ColumnInfo { Index = 9, Name = "當下位置", Width = 80 };
            public static readonly ColumnInfo Remark = new ColumnInfo { Index = 10, Name = "備註", Width = 200 };
            public static readonly ColumnInfo CarrierType = new ColumnInfo { Index = 11, Name = "類別", Width = 100 };

            public static void GridSetLocRange(ref DataGridView oGrid)
            {
                oGrid.ColumnCount = 12;
                oGrid.RowCount = 0;
                clInitSys.SetGridColumnInit(CmdSno, ref oGrid);
                clInitSys.SetGridColumnInit(CmdSts, ref oGrid);
                clInitSys.SetGridColumnInit(prty, ref oGrid);
                clInitSys.SetGridColumnInit(Source, ref oGrid);
                clInitSys.SetGridColumnInit(Destination, ref oGrid);
                clInitSys.SetGridColumnInit(CmdMode, ref oGrid);
                clInitSys.SetGridColumnInit(CrtDate, ref oGrid);
                clInitSys.SetGridColumnInit(ExpDate, ref oGrid);
                clInitSys.SetGridColumnInit(EndDate, ref oGrid);
                clInitSys.SetGridColumnInit(CurLoc, ref oGrid);
                clInitSys.SetGridColumnInit(Remark, ref oGrid);
                clInitSys.SetGridColumnInit(CarrierType, ref oGrid);
            }
        }
    }
}
