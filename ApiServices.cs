using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using vMonitor.UrlHistory;

namespace vMonitor
{
    internal class ApiServices
    {

        public class Result
        {
            public string Status { get; set; }
            public string Data { get; set; }    
        }

        public static Result RegisterMonitor()
        {

            var webAddr = Constants.BASE_VMONITOR_URL + "?browser=iexplorer";
            Debug.WriteLine("Register monitor URL " + webAddr);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var json = streamReader.ReadToEnd();
                Debug.WriteLine("Result data " + json);

                Result result = JsonConvert.DeserializeObject < Result>(json);

                return result;
            }
        }

        public static void SendHistoryToValsto(String email, String vMonitorId, List<STATURL> urls)
        {
            if (urls.Any())
            {
                var webAddr = Constants.BASE_VMONITOR_URL + "/" + vMonitorId + "/data";
                Debug.WriteLine("Send Data to " + webAddr);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    StringBuilder sb = new StringBuilder();
                    for(int i =0; i<urls.Count; i++)
                    {
                        var item = urls[i];
                        if (i > 0)
                            sb.Append(",");
                        sb.Append("{\"lastVisitTime\":" + item.ftLastVisited.dwHighDateTime + ", \"url\":\"" + item.URL + "\",\"visitCount\":1}");

                    }
                    string json = "{\"monitorId\":\"" + vMonitorId + "\", \"login\":"+email+",\"history\":["+sb.ToString() +"]}";
                    Debug.WriteLine("Send Data " + json);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var json = streamReader.ReadToEnd();
                    Debug.WriteLine("Result data " + json);
                }
            }
          
        }
    }
}