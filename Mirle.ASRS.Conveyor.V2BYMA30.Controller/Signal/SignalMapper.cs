using Mirle.MPLC;
using Mirle.MPLC.DataType;
using System.Collections.Generic;
using Mirle.Structure;
using Mirle.MPLC.DataBlocks;
using Mirle.MPLC.DataBlocks.DeviceRange;

namespace Mirle.ASRS.Conveyor.V2BYMA30_3F.Signal
{
    public class SignalMapper
    {
        public static readonly List<BlockInfo> SignalBlocks = new List<BlockInfo>()
        {
            new BlockInfo(new DDeviceRange("D1001", "5000"), "ReadWrite", 0),
        };

        public readonly static int BufferCount = 4;

        private readonly IMPLCProvider _mplc;
        private readonly Dictionary<int, BufferSignal> _Buffers = new Dictionary<int, BufferSignal>();
        private readonly ConveyorSignal _Signal = new ConveyorSignal();

        public SignalMapper(IMPLCProvider mplc)
        {
            _mplc = mplc;

            MappingConveyor();
            MappingBuffer();
        }

        public ConveyorSignal GetConveyorSignal()
        {
            return _Signal;
        }

        public BufferSignal GetBufferSignal(int bufferIndex)
        {
            _Buffers.TryGetValue(bufferIndex, out var buffer);
            return buffer;
        }

        private void MappingConveyor()
        {
            int addr = 1000;
            _Signal.Heartbeat = new Word(_mplc, $"D{addr + 1}");

            _Signal.AlarmBit = new BufferAlarmBitSignal();

            for (int Err = 0; Err < 16; Err++)
            {
                _Signal.AlarmBit.AlarmBit[Err].Checked = new Bit(_mplc, $"D{addr + 8}.{BitValue.BitOrder[Err]}");
            }

            addr = 3000;
            _Signal.Controller = new ConveyorControllerSignal
            {
                Heartbeat = new Word(_mplc, $"D{addr + 1}"),
                SystemTimeCalibration = new WordBlock(_mplc, $"D{addr + 2}", 6),
                Path = new Word(_mplc, $"D{addr + 8}"),
                ErrorIndex = new Word(_mplc, $"D{addr + 10}"),
            };
        }

        private void MappingBuffer()
        {
            int startAddress = 1010;
            int controllerStartAddress = startAddress + 2000;
            int bcrStartAddress_Tray = 5000;
            //int bcrStartAddress_LittleThing = 1700;
            //int bcrStartAddress_FOB = 1900;
            for (int i = 1; i <= BufferCount; i++)
            {
                int offset = (i - 1) * 10;
                var bufferSignal = new BufferSignal(i);
                bufferSignal.CommandID = new Word(_mplc, $"D{startAddress + offset + 1}");
                bufferSignal.CommandMode = new Word(_mplc, $"D{startAddress + offset + 2}");

                bufferSignal.StatusSignal = new BufferStatusSignal
                {
                    InMode = new Bit(_mplc, $"D{startAddress + offset + 3}.1"),
                    OutMode = new Bit(_mplc, $"D{startAddress + offset + 3}.2"),
                    Error = new Bit(_mplc, $"D{startAddress + offset + 3}.3"),
                    Auto = new Bit(_mplc, $"D{startAddress + offset + 3}.5"),
                    Manual = new Bit(_mplc, $"D{startAddress + offset + 3}.6"),
                    Presence = new Bit(_mplc, $"D{startAddress + offset + 3}.4"),
                    Position = new Bit(_mplc, $"D{startAddress + offset + 3}.8"),
                    Finish = new Bit(_mplc, $"D{startAddress + offset + 3}.9"),
                    EMO = new Bit(_mplc, $"D{startAddress + offset + 3}.A"),
                    //Online = new Bit(_mplc, $"D{startAddress + offset + 3}.B"),
                    //Offline = new Bit(_mplc, $"D{startAddress + offset + 3}.C")
                };

                bufferSignal.Ready = new Word(_mplc, $"D{startAddress + offset + 4}");
                bufferSignal.RollNotice = new Word(_mplc, $"D{startAddress + offset + 9}");
                //bufferSignal.IniNotice = new Word(_mplc, $"D{startAddress + offset + 10}");

                bufferSignal.Controller = new BufferControllerSignal();
                bufferSignal.Controller.CommandID = new Word(_mplc, $"D{controllerStartAddress + offset + 1}");
                bufferSignal.Controller.CommandMode = new Word(_mplc, $"D{controllerStartAddress + offset + 2}");
                //bufferSignal.Controller.PathNotice = new Word(_mplc, $"D{controllerStartAddress + offset + 6}");
                //bufferSignal.Controller.Manual_Putaway = new Word(_mplc, $"D{controllerStartAddress + offset + 7}");
                //bufferSignal.Controller.Pickup_Word = new Word(_mplc, $"D{controllerStartAddress + offset + 8}");
                bufferSignal.Controller.Pickup = new BufferPickupAckControllerSignal();
                bufferSignal.Controller.PickupFinish_Req = new BufferPickupAckControllerSignal();
                bufferSignal.PickupFinish_Ack = new BufferPickupAckControllerSignal();
                for (int pickup = 0; pickup < 16; pickup++)
                {
                    bufferSignal.Controller.Pickup.PickupBit[pickup].Checked = new Bit(_mplc, $"D{controllerStartAddress + offset + 8}.{BitValue.BitOrder[pickup]}");
                    bufferSignal.Controller.PickupFinish_Req.PickupBit[pickup].Checked = new Bit(_mplc, $"D{controllerStartAddress + offset +9}.{BitValue.BitOrder[pickup]}");

                    bufferSignal.PickupFinish_Ack.PickupBit[pickup].Checked = new Bit(_mplc, $"D{startAddress + offset + 9}.{BitValue.BitOrder[pickup]}");
                }

                //bufferSignal.Controller.HWS = new Bit(_mplc, $"D{controllerStartAddress + offset + 8}.D");
                //bufferSignal.Controller.FOB = new Bit(_mplc, $"D{controllerStartAddress + offset + 8}.E");

                bufferSignal.AckSignal = new BufferAckSignal
                {
                    InitalAck = new Word(_mplc, $"D{startAddress + offset + 10}"),
                    ReadBcrAck = new Word(_mplc, $"D{startAddress + offset + 5}")
                };

                bufferSignal.RequestController = new BufferRequestControllerSignal
                {
                    InitalReq = new Word(_mplc, $"D{controllerStartAddress + offset + 10}"),
                    ReadBcrReq = new Word(_mplc, $"D{controllerStartAddress + offset + 5}")
                };

                bufferSignal.AlarmBitSignal = new BufferAlarmBitSignal();
                bufferSignal.AlarmBitSignal_2 = new BufferAlarmBitSignal();
                for (int Err = 0; Err < 16; Err++)
                {
                    bufferSignal.AlarmBitSignal.AlarmBit[Err].Checked = new Bit(_mplc, $"D{startAddress + offset + 7}.{BitValue.BitOrder[Err]}");
                    bufferSignal.AlarmBitSignal_2.AlarmBit[Err].Checked = new Bit(_mplc, $"D{startAddress + offset + 8}.{BitValue.BitOrder[Err]}");
                }

                bufferSignal.BCRResultSignal = new BcrResultSignal();
                bufferSignal.BCRResultSignal.TrayID = new BCRResult();
                bufferSignal.BCRResultSignal.TrayID.ID = new WordBlock(_mplc, $"D{bcrStartAddress_Tray + clsTool.GetOffset_TrayID(i) + 1}", 5);

                //for (int Loc = 0; Loc < 8; Loc++)
                //{
                //    int idx = Loc * 6;
                //    try
                //    {
                //        bufferSignal.BCRResultSignal.GetLittleThingBcrByNum(Loc + 1).ID = new WordBlock(_mplc, $"D{bcrStartAddress_LittleThing + clsTool.GetOffset_LittleThings(i) + idx + 1}", 6);
                //    }
                //    catch { }

                //    if (Loc < 2)
                //    {
                //        try
                //        {
                //            bufferSignal.BCRResultSignal.GetFobBcrByNum(Loc + 1).ID = new WordBlock(_mplc, $"D{bcrStartAddress_FOB + clsTool.GetOffset_FOB(i) + idx + 1}", 6);
                //        }
                //        catch { }
                //    }
                //}

                _Buffers.Add(i, bufferSignal);
            }
        }
    }
}
