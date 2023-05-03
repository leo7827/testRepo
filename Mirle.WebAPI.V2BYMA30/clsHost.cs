using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirle.Def;
using Mirle.WebAPI.V2BYMA30.Function;

namespace Mirle.WebAPI.V2BYMA30
{
    public class clsHost
    {
        private WebApiConfig _config = new WebApiConfig();        
        private PositionReport positionReport;            
        private CV_BUFFER_QUERY BUFFER_QUERY;
        private BCR_CHECK_REQUEST bcr_check_request;
        private CMD_DESTINATION_CHECK cmd_destination_check;
        private ALARM_HAPPEN_REPORT alarm_happen_report;
        private CallEmptyReport callemptyReport;
        public clsHost(WebApiConfig Config)
        {
            _config = Config;           
            positionReport = new PositionReport(_config);
            BUFFER_QUERY = new CV_BUFFER_QUERY(_config);
            bcr_check_request = new BCR_CHECK_REQUEST(_config);
            alarm_happen_report = new ALARM_HAPPEN_REPORT(_config);
            cmd_destination_check = new CMD_DESTINATION_CHECK(_config);
            callemptyReport = new CallEmptyReport(_config);
        }

        public CMD_DESTINATION_CHECK GetCMD_DESTINATION_CHECK()
        {
            return cmd_destination_check;
        }


        public BCR_CHECK_REQUEST GetBCR_CHECK_REQUEST()
        {
            return bcr_check_request;
        }



        public ALARM_HAPPEN_REPORT GetAlarmReport()
        {
            return alarm_happen_report;
        }

    

        public PositionReport GetPositionReport()
        {
            return positionReport;
        }

        public CallEmptyReport GetCallEmptyReport()
        {
            return callemptyReport;
        }


        public CV_BUFFER_QUERY GetBUFFER_QUERY()
        {
            return BUFFER_QUERY;
        }

       

        
    }
}
