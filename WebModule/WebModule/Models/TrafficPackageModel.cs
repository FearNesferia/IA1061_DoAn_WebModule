using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebModule.Models
{
    public class TrafficPackageModel
    {
        public string WebsiteUrl { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Payload { get; set; }

        public override string ToString()
        {
            return $"{{ WebsiteUrl:\"{WebsiteUrl}\", Path:\"{Path}\", QueryString:\"{QueryString}\", Payload:\"{Payload}\" }}";
        }

        public Dictionary<string, string> ConvertToJsonValue()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("WebsiteUrl", WebsiteUrl);
            result.Add("Path", Path);
            result.Add("QueryString", QueryString);
            result.Add("Payload", Payload);
            return result;
        }
    }
}
