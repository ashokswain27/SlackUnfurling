using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlackUnfurling
{
    public interface ILogging
    {
        void LogError(string message);
        void LogError(Exception ex);
        void LogInfo(string message);
        void LogEvent(string message);
    }
}