using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebModule.Models;
using System.Runtime.Serialization.Json;
namespace WebModule
{
    public class IISModule : IHttpModule
    {
        private HttpClient client;
        private static bool isDetectMode = true;
        private string websiteUrl;
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var wrapper = new EventHandlerTaskAsyncHelper(SendRequest);
            context.AddOnPreRequestHandlerExecuteAsync(wrapper.BeginEventHandler, wrapper.EndEventHandler);
        }


        private async Task SendRequest(Object sender, EventArgs taget)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpRequest request = context.Request;
            string path = request.Url.Authority + request.Url.AbsolutePath;
            //websiteUrl = request.Url.Authority;
            websiteUrl = "fearnesferia.ddns.net:5000";
            websiteUrl = "testlocal";
            string s = request.Url.Query;
            string queryString = string.IsNullOrEmpty(s) ? "" : s.Substring(1);
            string payload;
            using (Stream receiveStream = request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    payload = readStream.ReadToEnd();
                }
            }
            TrafficPackageModel tpm = new TrafficPackageModel()
            {
                WebsiteUrl = websiteUrl,
                Path = path,
                QueryString = queryString,
                Payload = payload,
            };

            EventLog log = new EventLog();
            log.Source = "Application";
            try
            {
                //var response = await client.PostAsync("http://fearnesferia.ddns.net/v1/api/TrafficPackages", new FormUrlEncodedContent(tpm.ConvertToJsonValue()));
                var response = await client.PostAsync("http://localhost:49940//v1/api/TrafficPackages", new FormUrlEncodedContent(tpm.ConvertToJsonValue()));
                string content = await response.Content.ReadAsStringAsync();
                ResponeContent responeContent = new ResponeContent(content);

                log.WriteEntry($"{responeContent.isAttack} - {responeContent.isDetectMode}");
                if (responeContent.isAttack)
                {
                    if (responeContent.isDetectMode)
                    {
                        log.WriteEntry("warning, an anomalous traffic comming, contact module admin");
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteEntry(ex.Message);
            }
            
            //log.WriteEntry(content);
        }
    }
}
