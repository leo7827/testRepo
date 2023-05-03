using System;
using System.Diagnostics;

namespace Mirle.R46YP320.STK
{
    public static class DaifukuSpec
    {
        public static string DateTimeToTIME(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue) return string.Empty;
            return dateTime.ToString("yyyyMMddhhmmssff");
        }

        public static DateTime TIMEToDateTime(this string time)
        {
            try
            {
                return DateTime.ParseExact(time, "yyyyMMddhhmmssff", null);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}-{e.StackTrace}");
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// For S5F1
        /// </summary>
        public enum ALCDType
        {
            NotUsed = 0,
            PersonalSafety = 1,
            EquipmentSafety = 2,
            ParameterControlWarning = 3,
            ParameterControlError = 4,
            IrrecoverableError = 5,
            EquipmentStatusWarning = 6,
            AttentionFlags = 7,
            DataIntegrity = 8,
        }

        public enum RCMD
        {
            RESUME,
            CANCEL,
            PAUSE,
            SCAN,
            PRIORITYUPDATE,
            PORTTYPECHG,
            ABORT,
            INSTALL,
            REMOVE,
            TRANSFER,
            TRANSFER_BATCH,
            RETRY,
            LOCSTATECHG,
        }

        public enum VID
        {
            //Status Variable ID for S1F3
            SV_ActiveCarriers = 51,
            SV_ActiveTransfers = 52,
            SV_ActiveZones = 53,
            SV_ActiveZones_2 = 375,
            SV_AlarmEnabled = 3,
            SV_AlarmsSet = 4,
            SV_Clock = 5,
            SV_ControlState = 6,
            SV_CurrentCraneStates = 267,
            SV_CurrEqPortStatus = 350,
            SV_CurrentPortStates = 118,
            SV_EnhancedCarriers = 81,
            SV_EnhancedTransfers = 83,
            SV_EventsEnabled = 7,
            SV_MonitoredCranes = 280,
            SV_PortTypes = 351,
            SV_SCState = 79,
            SV_ShelfAllStats = 256,
            SV_SpecVersion = 114,
            SV_UnitAlarmStatList = 254,

            //Variable ID for S6F11
            DV_AlarmID = 1,
            DV_AlarmText = 212,
            DV_AllEnhancedDisableLocations = 379,
            DV_CarrierID = 54,
            DV_CarrierInfo = 55,
            DV_CarrierLoc = 56,
            DV_CarrierLocations = 94,
            DV_CarrierState = 203,
            DV_CarrierZoneName = 90,
            DV_CommandName = 57,
            DV_CommandID = 58,
            DV_CommandInfo = 59,
            DV_CommandType = 80,
            DV_Dest = 60,
            DV_DisabledLocations = 376,
            DV_EnhancedDisabledLocations = 378,
            DV_EmptyCarrier = 61,
            DV_EnhancedTransferCommand = 205,
            DV_EnhancedZoneData = 356,
            DV_EqPresenceStatus = 353,
            DV_EqReqStatus = 352,
            DV_ErrorID = 63,
            DV_HandoffType = 64,
            DV_IDReadStatus = 65,
            DV_InstallTime = 204,
            DV_MainteState = 257,
            DV_PortID = 115,
            DV_PortType = 66,
            DV_Priority = 67,
            DV_RecoveryOptions = 68,
            DV_ResultCode = 69,
            DV_Source = 70,
            DV_StockerCraneID = 88,
            DV_StockerUnitID = 71,
            DV_StockerUnitInfo = 72,
            DV_StockerUnitState = 73,
            DV_TransferCommand = 74,
            DV_TransferInfo = 75,
            DV_TransferState = 202,
            DV_ZoneCapacity = 76,
            DV_ZoneData = 77,
            DV_ZoneName = 78,
            DV_ZoneTotalSize = 377,
            DV_CstSize = 370,
            DV_PauseReason = 301,

            //ECID
            EC_EqpName = 62,
            EC_EstablishCommunicationTimeout = 2,
            EC_IDReadDuplicateOption = 111,
            EC_IDRFailuerOption = 112,
            EC_IDReadMismatchOption = 113,
        }

        public class ACK
        {
            /// <summary>
            /// For S2F42/S2F50
            /// </summary>
            public enum HCACK
            {
                Acknowledge = 0,
                CommandDoesNotExist = 1,
                CannotPerformNow = 2,
                AtLeastOneParameterIsInvalid = 3,
                AcknowledgeLaterPerformed = 4,
                Rejected = 5,
                NoSuchObjectExists = 6,
            }
        }

        public enum CEID
        {
            //Control Events

            EquipmentOffline = 1,
            ControlStatusLocal = 2,
            ControlStatusRemote = 3,

            //SC Transition Events

            AlarmCleared = 51,
            AlarmSet = 52,
            SCAutoCompleted = 53,
            SCAutoInitiated = 54,
            SCPauseCompleted = 55,
            SCPaused = 56,
            SCPauseInitiated = 57,

            //TRANSFER Command Status Transition Events

            TransferAbortCompleted = 101,
            TransferAbortFailed = 102,
            TransferAbortInitiated = 103,

            TransferCancelCompleted = 104,
            TransferCancelFailed = 105,
            TransferCancelInitiated = 106,

            TransferCompleted = 107,
            TransferInitiated = 108,

            TransferPaused = 109,
            TransferResumed = 110,

            PriorityUpdateCompleted = 662,
            PriorityUpdateFailed = 663,

            ScanInitiated = 681,
            ScanCompleted = 682,

            //Carrier Status Transition Events

            CarrierInstallCompleted = 151,
            CarrierRemovedCompleted = 152,
            CarrierRemoved = 153,
            CarrierResumed = 154,
            CarrierStored = 155,
            CarrierStoredAlt = 156,
            CarrierTransferring = 157,
            CarrierWaitIn = 158,
            CarrierWaitOut = 159,
            ZoneCapacityChange = 160,
            CarrierArrived = 161,

            CraneActive = 201,
            CraneIdle = 202,

            CraneOutOfService = 530,
            CraneInService = 531,
            ForkingStarted = 811,
            ForkingCompleted = 812,
            ForkRised = 813,
            ForkDowned = 814,
            CraneArrived = 820,

            //Port Status Transition Events

            PortOutOfService = 260,
            PortInService = 261,

            PortTypeInput = 603,
            PortTypeOutput = 604,
            PortTypeChanging = 605,

            //Load/Unload Status Events

            EqNoRequest = 606,
            EqLoadRequest = 607,
            EqUnLoadRequest = 608,

            EqPresence = 609,
            EqNoPresence = 610,

            ZoneShelfStateChanged = 627,

            //Unit Status Events

            UnitAlarmSet = 504,
            UnitAlarmCleared = 503,

            //Other Events
            CarrierIDRead = 251,
            CassetteIDRead = 271,
            IDReadError = 253,
            OperatorInitiatedAction = 254,
            DivergenceFailed = 270,
            CassetteDivergenceFailed = 545,
            EarthquakeDetection = 701,
        }
    }
}