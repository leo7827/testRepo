using Mirle.LCS.Models;
using Mirle.Stocker.TaskControl.Info;
using Mirle.Structure.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.R46YP320.STK.TaskService
{
    public class Scenario
    {
        public void GetScanScenario(string StockerID, string CraneID, string Task_CarrierID, ref string bcrResult, ref string reportCSTID, ref bool reportCarrierRemove, ref bool reportCarrierInstall, bool isDuplicate, ref VIDEnums.IDReadStatus IDReadStatus)
        {
            if (string.IsNullOrWhiteSpace(Task_CarrierID))
            {
                switch (bcrResult)
                {
                    case "ERROR1":
                    case "NORD01":
                    case "":
                        var abnormalCSTID = GetAbnormalCSTID(AbnormalCSTIDType.Failure, CraneID, "", StockerID);
                        reportCarrierInstall = true;
                        IDReadStatus = VIDEnums.IDReadStatus.Failure;
                        reportCSTID = abnormalCSTID;
                        break;
                    case "NOCST1":
                        bcrResult = "";
                        reportCSTID = "";
                        IDReadStatus = VIDEnums.IDReadStatus.NoCarrier;
                        break;
                    default:
                        if (string.IsNullOrEmpty(bcrResult))
                            break;
                        //沒帳  直接建帳
                        reportCarrierInstall = true;
                        IDReadStatus = isDuplicate ? VIDEnums.IDReadStatus.Duplicate : VIDEnums.IDReadStatus.Successful;
                        break;
                }
            }
            else
            {
                switch (bcrResult)
                {
                    case "ERROR1":
                    case "NORD01":
                    case "":
                        var abnormalCSTID = GetAbnormalCSTID(AbnormalCSTIDType.Failure, CraneID, Task_CarrierID, StockerID);
                        if (abnormalCSTID != Task_CarrierID)
                        {
                            reportCarrierRemove = true;
                            reportCarrierInstall = true;
                        }
                        IDReadStatus = VIDEnums.IDReadStatus.Failure;
                        reportCSTID = abnormalCSTID;
                        break;
                    case "NOCST1":
                        reportCSTID = "";
                        reportCarrierRemove = true;
                        IDReadStatus = VIDEnums.IDReadStatus.NoCarrier;
                        break;

                    default:
                        if (string.IsNullOrEmpty(bcrResult) || bcrResult == Task_CarrierID)
                            break;

                        //有帳 除帳  建帳
                        reportCarrierRemove = true;
                        reportCarrierInstall = true;
                        IDReadStatus = isDuplicate ? VIDEnums.IDReadStatus.Duplicate : VIDEnums.IDReadStatus.Mismatch;
                        break;
                }
            }
        }


        public string GetAbnormalCSTID(AbnormalCSTIDType abnormalCSTIDType, string hostEQPort, string CarrierID, string StockerID, string ShelfID = "")
        {
            string strNewCSTID = string.Empty;
            string strTrnDate = DateTime.Now.ToString("yyMMddHHmmssfff");

            if (abnormalCSTIDType == AbnormalCSTIDType.Failure && (CarrierID.StartsWith("UNKNOWN-") || CarrierID.Contains("UNKNOWNDUP") || CarrierID.Contains("UNKNOWNDBS") || CarrierID.Contains("UNKNOWNEMP")))
                return CarrierID;
            else
            {
                switch (abnormalCSTIDType)
                {
                    case AbnormalCSTIDType.Failure:
                        var oldCST = string.IsNullOrWhiteSpace(CarrierID) ? "000000" : CarrierID;
                        strNewCSTID = $"UNKNOWN-" + oldCST + "-" + hostEQPort + "-" + strTrnDate;
                        break;

                    case AbnormalCSTIDType.Duplicate:
                        strNewCSTID = $"UNKNOWNDUP-" + CarrierID + "-" + strTrnDate;
                        break;
                    case AbnormalCSTIDType.DoubleStorage:
                        strNewCSTID = $"UNKNOWNDBS-" + StockerID + ShelfID + "-" + strTrnDate;
                        break;
                    case AbnormalCSTIDType.EmptyRetrieval:
                    case AbnormalCSTIDType.NoCarrier:
                        if (CarrierID.StartsWith("UNKNOWNEMP-"))
                        {
                            strNewCSTID = CarrierID;
                        }
                        else
                        {
                            //只要不是同異常的UNK, 再次異常的話, 中間的CSTID改成 "UNKNOW"
                            if (!CarrierID.StartsWith("UNKNOWNEMP") && CarrierID.StartsWith("UNK"))
                                CarrierID = "UNKNOW-";
                            strNewCSTID = "UNKNOWNEMP-" + CarrierID + "-" + strTrnDate;
                        }
                        //strNewCSTID = $"UNKNOWNEMP-" + CarrierID + "-" + strTrnDate;
                        break;
                    default:
                        strNewCSTID = CarrierID;
                        break;
                }
            }
            return strNewCSTID;
        }
    }
}
