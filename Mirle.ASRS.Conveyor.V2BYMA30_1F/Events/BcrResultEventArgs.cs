using System;

namespace Mirle.ASRS.Conveyor.V2BYMA30_1F.Events
{
    public class BcrResultEventArgs : EventArgs
    {
        //public int BcrResultIndex { get; }
        public string BoxId { get; }

        //public BcrResultEventArgs(int bcrResultIndex, string boxId)
        //{
        //    BcrResultIndex = bcrResultIndex;
        //    BoxId = boxId;
        //}
        public BcrResultEventArgs(string boxId)
        {
            BoxId = boxId;
        }
    }
}
