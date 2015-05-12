using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer
{
    // Спасибо http://www.theforce.dk/hearthstone/ за проделанную работу
    public class Connection
    {
        public string srcIp { get; private set; }
        public ushort srcPort { get; private set; }
        public string dstIp { get; private set; }
        public ushort dstPort { get; private set; }

        public Connection(string srcIp, ushort srcPort, string dstIp, ushort dstPort)
        {
            this.srcIp = srcIp;
            this.srcPort = srcPort;
            this.dstIp = dstIp;
            this.dstPort = dstPort;
        }

        public Connection(PacketDotNet.TcpPacket packet)
        {
            srcIp = (packet.ParentPacket as PacketDotNet.IPv4Packet).SourceAddress.ToString();
            dstIp = (packet.ParentPacket as PacketDotNet.IPv4Packet).DestinationAddress.ToString();
            srcPort = (ushort)packet.SourcePort;
            dstPort = (ushort)packet.DestinationPort;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Connection))
                return false;
            Connection tcpClient = (Connection)obj;
            return
                tcpClient.srcPort == srcPort && tcpClient.dstPort == dstPort && tcpClient.srcIp.Equals(srcIp) && tcpClient.dstIp.Equals(dstIp)
                ||
                tcpClient.srcPort == dstPort && tcpClient.dstPort == srcPort && tcpClient.srcIp.Equals(dstIp) && tcpClient.dstIp.Equals(srcIp);
        }

        public override int GetHashCode()
        {
            return ((srcIp.GetHashCode() ^ srcPort.GetHashCode()) as object).GetHashCode() ^
                ((dstIp.GetHashCode() ^ dstPort.GetHashCode()) as object).GetHashCode();
        }
    }
}
