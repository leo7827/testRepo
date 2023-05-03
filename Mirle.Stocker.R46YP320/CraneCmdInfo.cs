using System;
using Mirle.Stocker.R46YP320.Extensions;

namespace Mirle.Stocker.R46YP320
{
    public enum CraneCmdType
    {
        MOVE,
        FROM,
        TO,
        FROM_TO,
        SCAN,
    }

    public enum CraneCmdForkType
    {
        Left,
        Right,
    }

    public enum NextStationType
    {
        STK1 = 0,
        STK2 = 1,
    }

    public class CraneCmdInfo
    {
        /// <summary>
        /// Only For CraneEventHandler OnTaskCmdWriteToMPLC
        /// </summary>
        public string CommandIdForEvent { get; set; } = string.Empty;

        /// <summary>
        /// Only For CraneEventHandler OnTaskCmdWriteToMPLC
        /// </summary>
        public string TaskNoForEvent { get; set; } = string.Empty;

        /// <summary>
        /// Only For CraneEventHandler OnTaskCmdWriteToMPLC
        /// </summary>
        public string CarrierIDForEvent { get; set; } = string.Empty;

        public int TaskNo { get; private set; }
        public CraneCmdType CmdType { get; set; } = CraneCmdType.MOVE;
        public bool BCREnable { get; set; }
        public CraneCmdForkType ForkType { get; set; } = CraneCmdForkType.Left;
        private int _fromLocation = 0;
        private int _toLocation = 10101;
        private int _batchId = 0;
        private string _cstid = "  ";
        private int _travelAxisSpeed = 20;
        private int _lifterAxisSpeed = 20;
        private int _rotateAxisSpeed = 20;
        private int _forkAxisSpeed = 20;
        private NextStationType _nextStation = 0;

        private readonly int _defaultCstIdLength = 20;
        private int _maxCstIdLength = 20;

        public CraneCmdInfo(int taskNo)
        {
            if (taskNo > 0 && taskNo <= 29999)
            {
                TaskNo = taskNo;
            }
            else
            {
                throw new InvalidOperationException("TaskNo Out of Range");
            }
        }

        public int FromLocation
        {
            get { return _fromLocation; }
            set
            {
                if (value >= 0 && value <= 65535)
                {
                    _fromLocation = value;
                }
            }
        }

        public int ToLocation
        {
            get { return _toLocation; }
            set
            {
                if (value >= 0 && value <= 65535)
                {
                    _toLocation = value;
                }
            }
        }

        public int BatchId
        {
            get { return _batchId; }
            set
            {
                if (value >= 0 && value <= 65535)
                {
                    _batchId = value;
                }
            }
        }

        public string CstId
        {
            get { return _cstid; }
            set { _cstid = string.IsNullOrEmpty(value?.Trim()) ? "  " : value.Trim().TruncateRight(_maxCstIdLength); }
        }

        public int MaxCstIdLength
        {
            get => _maxCstIdLength;
            set

            {
                if (value <= 0 || value > _defaultCstIdLength)
                {
                    _maxCstIdLength = _defaultCstIdLength;
                }
                else
                {
                    _maxCstIdLength = value;
                }
            }
        }

        public int TravelAxisSpeed
        {
            get { return _travelAxisSpeed; }
            set
            {
                if (value <= 20) _travelAxisSpeed = 20;
                else if (value >= 100) _travelAxisSpeed = 100;
                else _travelAxisSpeed = value;
            }
        }

        public int LifterAxisSpeed
        {
            get { return _lifterAxisSpeed; }
            set
            {
                if (value <= 20) _lifterAxisSpeed = 20;
                else if (value >= 100) _lifterAxisSpeed = 100;
                else _lifterAxisSpeed = value;
            }
        }

        public int RotateAxisSpeed
        {
            get { return _rotateAxisSpeed; }
            set
            {
                if (value <= 20) _rotateAxisSpeed = 20;
                else if (value >= 100) _rotateAxisSpeed = 100;
                else _rotateAxisSpeed = value;
            }
        }

        public int ForkAxisSpeed
        {
            get { return _forkAxisSpeed; }
            set
            {
                if (value <= 20) _forkAxisSpeed = 20;
                else if (value >= 100) _forkAxisSpeed = 100;
                else _forkAxisSpeed = value;
            }
        }

        public NextStationType NextStation
        {
            get => _nextStation;
            set => _nextStation = value;
        }
    }
}
