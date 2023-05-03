using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F
{
    public class clsTool
    {
        public static int GetOffset_TrayID(int BufferIndex)
        {
            switch (BufferIndex)
            {
                case 2:
                    return 0;
                case 7:
                    return 5;                             
                default:
                    return 60;
            }
        }
         

        /// <summary>
        /// 將數值轉成列舉值
        /// </summary>
        /// <typeparam name="TEnum">列舉Type</typeparam>
        /// <param name="EnumAsString">列舉數值</param>
        /// <returns>傳回列舉值</returns>
        public static TEnum funGetEnumValue<TEnum>(int EnumAsInt) where TEnum : struct
        {
            TEnum enumType = (TEnum)Enum.GetValues(typeof(TEnum)).GetValue(0);
            Enum.TryParse<TEnum>(EnumAsInt.ToString(), out enumType);
            return enumType;
        }

        public static void Signal_Show(bool bFlag, ref Label label)
        {
            if (bFlag == true) label.BackColor = Color.Yellow;
            else label.BackColor = Color.Transparent;
        }
    }
}
