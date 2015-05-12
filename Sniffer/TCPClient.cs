using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer
{
    internal class TcpClient
    {
        public Client teraClient {get; private set;}
        public Connection connection { get; private set; }

        public TcpClient(Connection connection)
        {
            // TODO: Complete member initialization
            this.connection = connection;
        }

        internal void reConstruction(PacketDotNet.TcpPacket tcpPacket)
        {
            throw new NotImplementedException();
        }
    }
}
