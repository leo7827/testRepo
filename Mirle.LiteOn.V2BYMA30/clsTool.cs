using System;
using Mirle.Structure;
using System.Linq;
using Mirle.Def.V2BYMA30;
using Mirle.Def;
using Mirle.ASRS.Conveyor.V2BYMA30_8F;
using System.Data;

namespace Mirle.LiteOn.V2BYMA30
{
    public class clsTool
    { 
         

          
        /// <summary>
        /// 將字串轉成列舉值
        /// </summary>
        /// <typeparam name="TEnum">列舉Type</typeparam>
        /// <param name="EnumAsString">列舉字串</param>
        /// <returns>傳回列舉值</returns>
        public static TEnum funGetEnumValue<TEnum>(string EnumAsString) where TEnum : struct
        {
            TEnum enumType = (TEnum)Enum.GetValues(typeof(TEnum)).GetValue(0);
            Enum.TryParse<TEnum>(EnumAsString, out enumType);
            return enumType;
        }
    }
}
