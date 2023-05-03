using Mirle.ASRS.WCS.DRCS.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.ASRS.WCS.DRCS.Manager
{
    public class CurrentMacroCommandManager
    {
        private readonly ConcurrentDictionary<string, MacroCommand> _MacroCommands = new ConcurrentDictionary<string, MacroCommand>();

        public CurrentMacroCommandManager()
        {
        }

        public void AddMacroCommand(MacroCommand macroCommand)
        {
            _MacroCommands.TryAdd(macroCommand.CommandID, macroCommand);
        }

        public void RemoveMacroCommand(string commandID)
        {
            _MacroCommands.TryRemove(commandID, out var macroCommand);
        }

        public bool GetMacroCommand(string commandID, out MacroCommand macroCommand)
        {
            return _MacroCommands.TryGetValue(commandID, out macroCommand);
        }

        public IEnumerable<MacroCommand> GetCurrentMacroCommands()
        {
            return _MacroCommands.Values;
        }
    }
}
