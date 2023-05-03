using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mirle.MPLC;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.DataType;
using Mirle.MPLC.FileData;
using Mirle.Stocker.R46YP320.View;

namespace Mirle.Stocker.R46YP320
{
    public class MPLCViewController : IMPLCViewController
    {
        private FileReader _fReader = new FileReader();
        private Form _defaultViewn;
        private Dictionary<Control, IDataType> _controlSignalMap = new Dictionary<Control, IDataType>();

        public MPLCViewController()
        {
            foreach (var block in SignalMapper4_11.SignalBlocks)
            {
                _fReader.AddDataBlock(new FileDataBlock(block.DeviceRange, columnIndex: block.PLCRawdataIndex));
            }
            _defaultViewn = new MonitorMainForm(_fReader, this);
        }

        public void SetMPLCSource(IMPLCProvider provider)
        {
        }

        public Form DefaultView()
        {
            return _defaultViewn;
        }

        public string Title { get; } = "CSOT Stocker MPLC Log Viewer";

        private IDataType _currentFocusedSignal = null;

        public IDataType CurrentFocusedSignal
        {
            get
            {
                var signal = _currentFocusedSignal;
                _currentFocusedSignal = null;
                return signal;
            }
            internal set
            {
                _currentFocusedSignal = value;
            }
        }

        public FileReader GetFileReader()
        {
            return _fReader;
        }

        public void MappingControlAndSignal(Control control, IDataType signal)
        {
            if (!_controlSignalMap.ContainsKey(control))
            {
                _controlSignalMap.Add(control, signal);
                control.Click += ControlOnClick;
            }
            else
            {
                _controlSignalMap[control] = signal;
            }
        }

        private void ControlOnClick(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                if (_controlSignalMap.TryGetValue(control, out var signal))
                {
                    CurrentFocusedSignal = signal;
                }
            }
        }
    }
}
