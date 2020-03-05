using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using RestSharp;


namespace SlackUnfurling
{
    public static class Utility
    {
        

        internal static string GetConfigValue(string Key)
        {
            string returnValue = null;

            returnValue =ConfigurationManager.AppSettings[Key];

            return returnValue;
        }

        
        public static string GetAppSetting(string appString)
        {
            return Environment.GetEnvironmentVariable(appString);
        }

        public static IRestResponse SendMessage(string ts, string channelName, string linkUrl, string pageTitle, string imageUrl, string articleContent)
        {
            var client = new RestClient("https://slack.com/api/chat.unfurl");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Length", "518");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Cookie", "__qca=P0-287712585-1556558598772; d=vUqGZs2a3l9JUwbuy%2BJ9tJGcL178GO%2BgZTFnR2wvajlXdzBqajFZZ0MrVTE1Y1U2VWVxSURtcU1wTkNvaWR0SkFsOG1BemU4NUlpT3IrSFltY2s5b3VSa3UyUU9peHlpYTNFT1lYNGMxNGZIVkwwS2k1TGZOb1MvNnduVWN0UUs4ckcrV1dWZDBQQ3ZRVHVZYlVlVm9lalRZN0p2dEtDWE9WOUEvK0VKL2c9PQiQe%2FY%2FI3l6TXpw4f0nwWU%3D; d-s=1556558913; _ga=GA1.2.886974099.1556558599; b=.356w4onc2v1s38u5nzj6ngiuq; _gcl_au=1.1.889099315.1566187098");
            request.AddHeader("Host", "slack.com");
            request.AddHeader("Postman-Token", "a5944168-0a04-4457-b1a3-2de66e77592d,c3a651f4-11f3-45bf-9b95-ff0ab88e1946");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("User-Agent", "PostmanRuntime/7.15.2");
            request.AddHeader("Content-Type", "application/json,multipart/form-data; boundary=--------------------------102615583454824705594555");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", "");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"channel\"\r\n\r\n" + channelName + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ts\"\r\n\r\n" + ts + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"unfurls\"\r\n\r\n{\"" + linkUrl + "\":{\"text\":\"" + articleContent + "\",\"image_url\":\"" + imageUrl + "\",\"title\":\"" + pageTitle + "\",\"title_link\":\"" + linkUrl + "\",\"color\":\"#E20074\"}}\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }
    }
}