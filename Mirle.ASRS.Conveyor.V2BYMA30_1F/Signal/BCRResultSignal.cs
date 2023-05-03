using Mirle.MPLC.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.Signal
{
    public class BcrResultSignal
    {
        private BCRResult[] LittleThings = new BCRResult[8];
        private BCRResult[] FOBs = new BCRResult[2];

        public BCRResult TrayID { get; internal set; }

        public BcrResultSignal()
        {
            for (int i = 0; i < LittleThings.Length; i++)
            {
                LittleThings[i] = new BCRResult();

                if(i < 2)
                {
                    FOBs[i] = new BCRResult();
                }
            }
        }

        public BCRResult GetLittleThingBcrByNum(int Num)
        {
            return LittleThings[Num - 1];
        }

        public BCRResult GetFobBcrByNum(int Num)
        {
            return FOBs[Num - 1];
        }
    }
}
