using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer
{
    internal class TcpClient
    {
        private Connection connection1;

        public TcpClient(Connection connection1)
        {
            // TODO: Complete member initialization
            this.connection1 = connection1;
        }
        public Client teraClient {get; private set;}
        public Connection connection { get; private set; }

        internal void reConstruction(PacketDotNet.TcpPacket tcpPacket)
        {
            throw new NotImplementedException();
        }
    }
}
