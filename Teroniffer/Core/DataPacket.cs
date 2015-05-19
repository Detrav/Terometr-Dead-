using Detrav.Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teroniffer.Core
{
    class DataPacket
    {
        public int num { get; set; }
        public TeraPacket.Type type { get; set; }
        public ushort size { get; set; }
        public OpCode2805 opCode { get; set; }
        private TeraPacket packet;
        public DataPacket(int num, TeraPacket packet)
        {
            this.packet = packet;
            this.num = num;
            this.type = packet.type;
            this.size = packet.size;
            this.opCode = (OpCode2805)packet.opCode;
        }

        public TeraPacket getTeraPacket() { return packet; }
    }
}
