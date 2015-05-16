using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Tera.P2805
{
    class S_SPAWN_PROJECTILE : TeraPacketParser
    {
        public S_SPAWN_PROJECTILE(TeraPacket packet) : base(packet)
        {
            readUInt64(4, "id");
            readUInt64(49, "player id");
        }
    }
}
