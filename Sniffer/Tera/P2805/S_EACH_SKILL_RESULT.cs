using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Tera.P2805
{
    class S_EACH_SKILL_RESULT : TeraPacketParser
    {
        public S_EACH_SKILL_RESULT(TeraPacket packet)
            : base(packet)
        {
            try
            {
                readUInt32("timesteps");
                readUInt64("attacker");
                readUInt64("attacked");
            }
            catch { }
        }
    }
}
