using Mirle.ASRS.WCS.DRCS.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.DRCS.Model
{
    public class MacroCommand
    {
        public string CommandID { get; }
        public IEnumerable<ICarrier> Carriers { get; }
        public MacroCommandStates MacroCommandState { get; private set; }

        public MacroCommand(string commandID, IEnumerable<ICarrier> carrier)
        {
            CommandID = commandID;
            Carriers = carrier;
        }

        public void SetMacroCommandState(MacroCommandStates macroCommandStates)
        {
            MacroCommandState = macroCommandStates;
        }
    }
}
