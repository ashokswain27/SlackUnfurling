using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using Microsoft.ApplicationInsights.DataContracts;

namespace SlackUnfurling
{
    public class Logging: ILogging
    {
      //  private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static TelemetryClient _Logger = new TelemetryClient();

        public Logging() { }

        public void LogError(string message)
        {
           // _Log.Error(new Exception(message));
            _Logger.TrackException(new Exception(message));
            Trace.WriteLine(message);
        }

        public void LogError(Exception ex)
        {
          //  _Log.Error(ex);
            _Logger.TrackException(ex);
            Trace.WriteLine(ex.Message);
        }

        public void LogInfo(string message)
        {
           // _Log.Info(message);
            _Logger.TrackTrace(message,SeverityLevel.Information);
            Trace.WriteLine(message);
        }

        public void LogEvent(string message)
        {
            _Logger.TrackEvent(message);
        }
    }
}