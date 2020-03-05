using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Threading.Tasks;

namespace SlackUnfurling.Controllers
{
    public class UnfurlingEventsController : ApiController
    {
        private static readonly string errorMessage = "We apologize but an unexpected error occured. Please try again later";
        private static readonly string badRequestErrorMessage = "We apologize but an unexpected error occured due to bad request";
        private static readonly string badInputErrorMessage = "We apologize but an unexpected error occured due to bad input or Unprocessable Entity";

        public static Logging _appInsLog = new Logging();
        public static string[] resultArr = new string[3];
        

        [HttpPost]
        //public async Task<IHttpActionResult> Event()
        public  IHttpActionResult Event([FromBody] JObject objInformationRequest)
        {
            try
            {               
                _appInsLog.LogInfo("Call Slack Event Post method");

                if (objInformationRequest != null)
                {
                    _appInsLog.LogInfo("Event Request Payload" + Convert.ToString(objInformationRequest));
                    if (Common.IsValidJson(Convert.ToString(objInformationRequest)))
                    {
                        dynamic data = JsonConvert.DeserializeObject(Convert.ToString(objInformationRequest));


                        var varStringcontent = new StringContent("");
                        var model = new { challenge = data.challenge };
                        string eventType = Convert.ToString(data.type);
                        _appInsLog.LogInfo("Event Request Type: " + eventType);

                        if (eventType.Equals("url_verification", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Ok(model);
                        }
                        else if (eventType.Equals("event_callback", StringComparison.InvariantCultureIgnoreCase))
                        {

                            var unfurlSuccess = new { ok = "true" };
                            string timeStamp = string.Empty;
                            string jsonLink = "";
                            string linksValue = "";
                            string channelName = "";

                            var eventDetails = data.Value<JObject>("event").Properties();

                            foreach (var slackEvent in eventDetails)
                            {
                                if (slackEvent.Name == "message_ts")
                                {
                                    timeStamp = slackEvent.Value;
                                }
                                if (slackEvent.Name == "links")
                                {
                                    jsonLink = slackEvent.Value.ToString();
                                }
                                if (slackEvent.Name == "channel")
                                {
                                    channelName = slackEvent.Value;
                                }
                            }
                            _appInsLog.LogInfo("Get Slack Event Request json-link: " + jsonLink);
                            JObject links = JObject.Parse(jsonLink.Replace("[", "").Replace("]", ""));
                            foreach (var link in links)
                            {
                                if (link.Key.ToString() == "url")
                                {
                                    linksValue = link.Value.ToString();
                                }
                            }

                          
                          
                            string clientSecret = Utility.GetAppSetting("SPOClintSecret");
                            GetSharePointFile(linksValue, clientSecret);
                            string pageTitle = resultArr[0];
                            string imageUrl = resultArr[1];
                            string articleContent = resultArr[2];


                            IRestResponse response =Utility.SendMessage(timeStamp, channelName, linksValue, pageTitle, imageUrl, articleContent);
                            if (response.IsSuccessful)
                            {
                                _appInsLog.LogInfo("slack event unfurl success for link: " + linksValue);
                            }
                            else
                            {
                                _appInsLog.LogInfo("slack event unfurl error for link : " + linksValue);
                            }
                            return Ok(true);
                        }
                        else
                        {
                            return Ok("No event type is registered in Slack yet");
                        }


                    }
                    else
                    {
                        return ResponseMessage(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = errorMessage, Content = new StringContent(badRequestErrorMessage) });
                    }
                }
                else
                {
                    return ResponseMessage(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = errorMessage, Content = new StringContent(badInputErrorMessage) });
                }
                  
               
            }
            catch(Exception ex)
            {
                _appInsLog.LogError("Slack Event Request Error:"+ex+"/r /n"+ex.StackTrace+"/r /n"+ex.Message);
                throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = errorMessage, Content = new StringContent(errorMessage) });
            }
        }

       

      

        public static string[] GetSharePointFile(string fileUrl, string clientSecret)
        {
            
            OfficeDevPnP.Core.AuthenticationManager authManager = new OfficeDevPnP.Core.AuthenticationManager();
            
            using (ClientContext context = authManager.GetAppOnlyAuthenticatedContext(Common.GetEnvironmentVariable("SiteUrl"), Common.GetEnvironmentVariable("ClientID"), clientSecret))
            {
                Web currentWeb = context.Web;
                Microsoft.SharePoint.Client.File file = currentWeb.GetFileByUrl(fileUrl);
                context.Load(file);
                context.ExecuteQuery();
                
                ListItem item = file.ListItemAllFields;
                context.Load(file.ListItemAllFields);
                context.ExecuteQuery();
                string imageField =Convert.ToString(item["OVFeatureImage"]);
                string articleImageUrl = string.Empty;
                

                if (!string.IsNullOrEmpty(imageField))
                {
                    //Regex to extract the src value from the img tag
                    string imageUrl = Regex.Match(imageField, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;
                    // SharePoint image Link url
                    articleImageUrl = Utility.GetAppSetting("SPOImageLink") + (imageUrl.Replace("/sites/intranet/PublishingImages", ""));
                }
                else
                {
                    // Keep a default image
                    articleImageUrl = Utility.GetAppSetting("SPOImageLink");
                }

                string tnationArticleIntro = Convert.ToString(item["TNationArticleIntro"]);
                
                resultArr[0]= file.Title;
                resultArr[1] = articleImageUrl;
                resultArr[2] = tnationArticleIntro;
                return resultArr;

            }
        }


      

        

      
    }
}
