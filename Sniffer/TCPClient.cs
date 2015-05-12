using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer
{
    internal class TcpClient
    {
        public string srcIp { get; private set; }
        public ushort srcPort { get; private set; }
        public string dstIp { get; private set; }
        public ushort dstPort { get; private set; }

        public TcpClient(string srcIp, ushort srcPort, string dstIp, ushort dstPort)
        {
            this.srcIp = srcIp;
            this.srcPort = srcPort;
            this.dstIp = dstIp;
            this.dstPort = dstPort;
        }
    }
}
