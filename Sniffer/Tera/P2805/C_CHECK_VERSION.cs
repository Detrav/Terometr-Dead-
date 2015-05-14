using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Tera.P2805
{
    class C_CHECK_VERSION : TeraPacketParser
    {
        public C_CHECK_VERSION(TeraPacket packet)
            : base(packet)
        {

        }
    }
}
