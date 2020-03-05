using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlackUnfurling.Models
{
    public class EventRequest
    {
        public string channel { get; set; }
        public string ts { get; set; }
        

        public string token { get; set; }

        public Dictionary<string, Object> unfurls { get; set; }
      //  public Unfurls unfurls { get; set; }

    }

    
    public class UlfurlText
    {
        public string text { get; set; }
    }


    //public class SlackEvent
}