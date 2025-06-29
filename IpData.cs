using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server
{
    internal class IpData
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public IpData()
        {
            IP = "127.0.0.1";
            Port = 6565;
        }
    }
}
