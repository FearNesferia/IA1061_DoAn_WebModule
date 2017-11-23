using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebModule.Models
{
    class ResponeContent
    {
        public bool isAttack { get; set; }
        public bool isDetectMode { get; set; }

        public ResponeContent(string content)
        {
            //{ "isAttack":true,"isDetectMode":true}
            content = content.Replace('{', ' ').Replace('}', ' ').Trim();
            List<string> s = content.Split(',').ToList();
            isAttack = s[0].Split(':')[1] == "true";
            isDetectMode = s[1].Split(':')[1] == "true";
        }

    }
}
