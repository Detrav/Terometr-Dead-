using Detrav.Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Teroniffer.Core
{
    class DataPacket
    {
        public int num { get; set; }
        public TeraPacket.Type type { get; set; }
        public ushort size { get; set; }
        public object opCode { get; set; }
        private TeraPacket packet;
        public DataPacket(int num, TeraPacket packet)
        {
            this.packet = packet;
            this.num = num;
            this.type = packet.type;
            this.size = packet.size;
            this.opCode = TeraPacketCreator.getOpCode(packet.opCode);
        }

        public TeraPacket getTeraPacket() { return packet; }
    }
}
