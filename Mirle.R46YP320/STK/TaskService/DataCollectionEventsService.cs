using System.Threading.Tasks;
using Mirle.R46YP320.STK.DataCollectionEventArgs;
using Mirle.Stocker.TaskControl.Module;

namespace Mirle.R46YP320.STK.TaskService
{
    public class DataCollectionEventsService : DataCollectionEventsModule
    {
        //Control Related Events
        public event DataCollectionEventHandler<ControlModeEventArgs> EquipmentOffline;
        public event DataCollectionEventHandler<ControlModeEventArgs> EquipmentRemote;
        public event DataCollectionEventHandler<ControlModeEventArgs> EquipmentLocal;

        //SC Transition Events
        public event DataCollectionEventHandler<AlarmClearedEventArgs> AlarmCleared;
        public event DataCollectionEventHandler<AlarmSetEventArgs> AlarmSet;
        public event DataCollectionEventHandler<SCStateEventArgs> SCAutoCompleted;
        public event DataCollectionEventHandler<SCStateEventArgs> SCAutoInitiated;
        public event DataCollectionEventHandler<SCStateEventArgs> SCPauseCompleted;
        public event DataCollectionEventHandler<SCStateEventArgs> SCPaused;
        public event DataCollectionEventHandler<SCInitEventArgs> SCPauseInitiated;

        //TRANSFER Command Status Transition Events
        public event DataCollectionEventHandler<TransferAbortCompletedEventArgs> TransferAbortCompleted;
        public event DataCollectionEventHandler<TransferAbortFailedEventArgs> TransferAbortFailed;
        public event DataCollectionEventHandler<TransferAbortInitiatedEventArgs> TransferAbortInitiated;
        public event DataCollectionEventHandler<TransferCancelCompletedEventArgs> TransferCancelCompleted;
        public event DataCollectionEventHandler<TransferCancelFailedEventArgs> TransferCancelFailed;
        public event DataCollectionEventHandler<TransferCancelInitiatedEventArgs> TransferCancelInitiated;
        public event DataCollectionEventHandler<TransferCompletedEventArgs> TransferCompleted;
        public event DataCollectionEventHandler<TransferInitiatedEventArgs> TransferInitiated;
        public event DataCollectionEventHandler<TransferPausedEventArgs> TransferPaused;
        public event DataCollectionEventHandler<TransferResumedEventArgs> TransferResumed;
        public event DataCollectionEventHandler<PriorityUpdateCompletedEventArgs> PriorityUpdateCompleted;
        public event DataCollectionEventHandler<PriorityUpdateFailedEventArgs> PriorityUpdateFailed;
        public event DataCollectionEventHandler<ScanInitiatedEventArgs> ScanInitiated;
        public event DataCollectionEventHandler<ScanCompletedEventArgs> ScanCompleted;

        //Carrier Status Transition Events
        public event DataCollectionEventHandler<CarrierInstallCompletedEventArgs> CarrierInstallCompleted;
        public event DataCollectionEventHandler<CarrierRemoveCompletedEventArgs> CarrierRemoveCompleted;
        public event DataCollectionEventHandler<CarrierRemovedEventArgs> CarrierRemoved;
        public event DataCollectionEventHandler<CarrierResumedEventArgs> CarrierResumed;
        public event DataCollectionEventHandler<CarrierStoredEventArgs> CarrierStored;
        public event DataCollectionEventHandler<CarrierStoredAltEventArgs> CarrierStoredAlt;
        public event DataCollectionEventHandler<CarrierTransferringEventArgs> CarrierTransferring;
        public event DataCollectionEventHandler<CarrierWaitInEventArgs> CarrierWaitIn;
        public event DataCollectionEventHandler<CarrierWaitOutEventArgs> CarrierWaitOut;
        public event DataCollectionEventHandler<ZoneCapacityChangeEventArgs> ZoneCapacityChange;
        public event DataCollectionEventHandler<CarrierArrivedEventArgs> CarrierArrived;

        //Crane Status Transition Events
        public event DataCollectionEventHandler<CraneStatusEventArgs> CraneIdle;
        public event DataCollectionEventHandler<CraneStatusEventArgs> CraneActive;
        public event DataCollectionEventHandler<CraneServiceStatusEventArgs> CraneOutOfService;
        public event DataCollectionEventHandler<CraneServiceStatusEventArgs> CraneInService;
        public event DataCollectionEventHandler<ForkStatusEventArgs> ForkingStarted;
        public event DataCollectionEventHandler<ForkStatusEventArgs> ForkingCompleted;
        public event DataCollectionEventHandler<ForkStatusEventArgs> ForkRised;
        public event DataCollectionEventHandler<ForkStatusEventArgs> ForkDowned;
        public event DataCollectionEventHandler<CraneArrivedEventArgs> CraneArrived;

        //Port Status Transition Events
        public event DataCollectionEventHandler<PortStateEventArgs> PortOutOfService;
        public event DataCollectionEventHandler<PortStateEventArgs> PortInService;
        public event DataCollectionEventHandler<PortStateEventArgs> PortTypeInput;
        public event DataCollectionEventHandler<PortStateEventArgs> PortTypeOutput;
        public event DataCollectionEventHandler<PortStateEventArgs> PortTypeChanging;
        public event DataCollectionEventHandler<PortStateEventArgs> EqNoRequest;
        public event DataCollectionEventHandler<PortStateEventArgs> EqLoadRequest;
        public event DataCollectionEventHandler<PortStateEventArgs> EqUnLoadRequest;
        public event DataCollectionEventHandler<PortStateEventArgs> EqPresence;
        public event DataCollectionEventHandler<PortStateEventArgs> EqNoPresence;
        public event DataCollectionEventHandler<ZoneShelfStateChangedEventArgs> ZoneShelfStateChanged;

        //Alarm Related Events
        public event DataCollectionEventHandler<UnitAlarmEventArgs> UnitAlarmSet;
        public event DataCollectionEventHandler<UnitAlarmEventArgs> UnitAlarmCleared;

        //Other Events
        public event DataCollectionEventHandler<CarrierIDReadEventArgs> CarrierIDRead;
        public event DataCollectionEventHandler<CassetteIDReadEventArgs> CassetteIDRead;
        public event DataCollectionEventHandler<IDReadErrorEventArgs> IDReadError;
        public event DataCollectionEventHandler<OperatorInitiatedActionEventArgs> OperatorInitiatedAction;
        public event DataCollectionEventHandler<DivergenceFailedEventArgs> DivergenceFailed;
        public event DataCollectionEventHandler<CassetteDivergenceFailedEventArgs> CassetteDivergenceFailed;
        public event DataCollectionEventHandler<AlarmReportEventArgs> AlarmReportSend;
        public event DataCollectionEventHandler<ControlModeEventArgs> EarthquakeDetection;

        //Control Related Events
        public void OnEquipmentOffline(object sender, ControlModeEventArgs args) =>  EquipmentOffline?.Invoke(sender, args);
        public void OnEquipmentRemote(object sender, ControlModeEventArgs args) =>  EquipmentRemote?.Invoke(sender, args);
        public void OnEquipmentLocal(object sender, ControlModeEventArgs args) =>  EquipmentLocal?.Invoke(sender, args);

        //SC Transition Events
        public void OnAlarmCleared(object sender, AlarmClearedEventArgs args) =>  AlarmCleared?.Invoke(sender, args);
        public void OnAlarmSet(object sender, AlarmSetEventArgs args) =>  AlarmSet?.Invoke(sender, args);
        public void OnSCAutoCompleted(object sender, SCStateEventArgs args) =>  SCAutoCompleted?.Invoke(sender, args);
        public void OnSCAutoInitiated(object sender, SCStateEventArgs args) =>  SCAutoInitiated?.Invoke(sender, args);
        public void OnSCPauseCompleted(object sender, SCStateEventArgs args) =>  SCPauseCompleted?.Invoke(sender, args);
        public void OnSCPaused(object sender, SCStateEventArgs args) =>  SCPaused?.Invoke(sender, args);
        public void OnSCPauseInitiated(object sender, SCInitEventArgs args) =>  SCPauseInitiated?.Invoke(sender, args);

        //TRANSFER Command Status Transition Events
        public void OnTransferAbortCompleted(object sender, TransferAbortCompletedEventArgs args) =>  TransferAbortCompleted?.Invoke(sender, args);
        public void OnTransferAbortFailed(object sender, TransferAbortFailedEventArgs args) =>  TransferAbortFailed?.Invoke(sender, args);
        public void OnTransferAbortInitiated(object sender, TransferAbortInitiatedEventArgs args) =>  TransferAbortInitiated?.Invoke(sender, args);
        public void OnTransferCancelCompleted(object sender, TransferCancelCompletedEventArgs args) =>  TransferCancelCompleted?.Invoke(sender, args);
        public void OnTransferCancelFailed(object sender, TransferCancelFailedEventArgs args) =>  TransferCancelFailed?.Invoke(sender, args);
        public void OnTransferCancelInitiated(object sender, TransferCancelInitiatedEventArgs args) =>  TransferCancelInitiated?.Invoke(sender, args);
        public void OnTransferPaused(object sender, TransferPausedEventArgs args) =>  TransferPaused?.Invoke(sender, args);
        public void OnTransferResumed(object sender, TransferResumedEventArgs args) =>  TransferResumed?.Invoke(sender, args);
        public void OnTransferCompleted(object sender, TransferCompletedEventArgs args) =>  TransferCompleted?.Invoke(sender, args);
        public void OnTransferInitiated(object sender, TransferInitiatedEventArgs args) =>  TransferInitiated?.Invoke(sender, args);
        public void OnPriorityUpdateCompleted(object sender, PriorityUpdateCompletedEventArgs args) => PriorityUpdateCompleted?.Invoke(sender, args);
        public void OnPriorityUpdateFailed(object sender, PriorityUpdateFailedEventArgs args) => PriorityUpdateFailed?.Invoke(sender, args);
        public void OnScanInitiated(object sender, ScanInitiatedEventArgs args) => ScanInitiated?.Invoke(sender, args);
        public void OnScanCompleted(object sender, ScanCompletedEventArgs args) => ScanCompleted?.Invoke(sender, args);

        //Carrier Status Transition Events
        public void OnCarrierInstallCompleted(object sender, CarrierInstallCompletedEventArgs args) =>  CarrierInstallCompleted?.Invoke(sender, args);
        public void OnCarrierRemovedCompleted(object sender, CarrierRemoveCompletedEventArgs args) =>  CarrierRemoveCompleted?.Invoke(sender, args);
        public void OnCarrierRemoved(object sender, CarrierRemovedEventArgs args) => CarrierRemoved?.Invoke(sender, args);
        public void OnCarrierResumed(object sender, CarrierResumedEventArgs args) => CarrierResumed?.Invoke(sender, args);
        public void OnCarrierStored(object sender, CarrierStoredEventArgs args) =>  CarrierStored?.Invoke(sender, args);
        public void OnCarrierStoredAlt(object sender, CarrierStoredAltEventArgs args) =>  CarrierStoredAlt?.Invoke(sender, args);
        public void OnCarrierTransferring(object sender, CarrierTransferringEventArgs args) =>  CarrierTransferring?.Invoke(sender, args);
        public void OnCarrierWaitIn(object sender, CarrierWaitInEventArgs args) => CarrierWaitIn?.Invoke(sender, args);
        public void OnCarrierWaitOut(object sender, CarrierWaitOutEventArgs args) => CarrierWaitOut?.Invoke(sender, args);
        public void OnZoneCapacityChange(object sender, ZoneCapacityChangeEventArgs args) => ZoneCapacityChange?.Invoke(sender, args);
        public void OnCarrierArrived(object sender, CarrierArrivedEventArgs args) => CarrierArrived?.Invoke(sender, args);

        //Crane Status Transition Events
        public void OnCraneActive(object sender, CraneStatusEventArgs args) => CraneActive?.Invoke(sender, args);
        public void OnCraneIdle(object sender, CraneStatusEventArgs args) =>  CraneIdle?.Invoke(sender, args);
        public void OnCraneOutOfService(object sender, CraneServiceStatusEventArgs args) => CraneOutOfService?.Invoke(sender, args);
        public void OnCraneInService(object sender, CraneServiceStatusEventArgs args) => CraneInService?.Invoke(sender, args);
        public void OnForkingStarted(object sender, ForkStatusEventArgs args) => ForkingStarted?.Invoke(sender, args);
        public void OnForkingCompleted(object sender, ForkStatusEventArgs args) => ForkingCompleted?.Invoke(sender, args);
        public void OnForkRised(object sender, ForkStatusEventArgs args) => ForkRised?.Invoke(sender, args);
        public void OnForkDowned(object sender, ForkStatusEventArgs args) => ForkDowned?.Invoke(sender, args);
        public void OnCraneArrived(object sender, CraneArrivedEventArgs args) => CraneArrived?.Invoke(sender, args);

        //Port Status Transition Events
        public void OnPortOutOfService(object sender, PortStateEventArgs args) => PortOutOfService?.Invoke(sender, args);
        public void OnPortInService(object sender, PortStateEventArgs args) => PortInService?.Invoke(sender, args);
        public void OnPortTypeInput(object sender, PortStateEventArgs args) => PortTypeInput?.Invoke(sender, args);
        public void OnPortTypeOutput(object sender, PortStateEventArgs args) => PortTypeOutput?.Invoke(sender, args);
        public void OnPortTypeChanging(object sender, PortStateEventArgs args) => PortTypeChanging?.Invoke(sender, args);
        public void OnEqNoRequest(object sender, PortStateEventArgs args) => EqNoRequest?.Invoke(sender, args);
        public void OnEqLoadRequest(object sender, PortStateEventArgs args) => EqLoadRequest?.Invoke(sender, args);
        public void OnEqUnLoadRequest(object sender, PortStateEventArgs args) => EqUnLoadRequest?.Invoke(sender, args);
        public void OnEqPresence(object sender, PortStateEventArgs args) => EqPresence?.Invoke(sender, args);
        public void OnEqNoPresence(object sender, PortStateEventArgs args) => EqNoPresence?.Invoke(sender, args);
        public void OnZoneShelfStateChanged(object sender, ZoneShelfStateChangedEventArgs args) => ZoneShelfStateChanged?.Invoke(sender, args);

        //Alarm Related Events
        public void OnUnitAlarmSet(object sender, UnitAlarmEventArgs args) =>  UnitAlarmSet?.Invoke(sender, args);
        public void OnUnitAlarmCleared(object sender, UnitAlarmEventArgs args) =>  UnitAlarmCleared?.Invoke(sender, args);

        //Other Events
        public void OnCarrierIDRead(object sender, CarrierIDReadEventArgs args) => CarrierIDRead?.Invoke(sender, args);
        public void OnCassetteIDRead(object sender, CassetteIDReadEventArgs args) => CassetteIDRead?.Invoke(sender, args);
        public void OnIDReadError(object sender, IDReadErrorEventArgs args) => IDReadError?.Invoke(sender, args);
        public void OnOperatorInitiatedAction(object sender, OperatorInitiatedActionEventArgs args) => OperatorInitiatedAction?.Invoke(sender, args);
        public void OnDivergenceFailed(object sender, DivergenceFailedEventArgs args) => DivergenceFailed?.Invoke(sender, args);
        public void OnCassetteDivergenceFailed(object sender, CassetteDivergenceFailedEventArgs args) => CassetteDivergenceFailed?.Invoke(sender, args);
        public void OnEarthquakeDetection(object sender, ControlModeEventArgs args) => EarthquakeDetection?.Invoke(sender, args);
        public void OnAlarmReportSend(object sender, AlarmReportEventArgs args) => AlarmReportSend?.Invoke(sender, args);
    }
}
