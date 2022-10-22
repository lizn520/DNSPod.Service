using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSPod.Service
{
    public class Record
    {
        public string subdomain { get; set; }
        public string domain { get; set; }
        public string update_v4 { get; set; }
        public string update_v6 { get; set; }
        public string ip { get; set; }
        public string _ip { get; set; }
        public string ipv6 { get; set; }
        public string _ipv6 { get; set; }
    }
}
