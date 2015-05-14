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
            try
            {
                int count = readUInt16("count");
                for (int i = 0; i < count; i++)
                {
                    readInt32("NowShift & NextShift");
                    readInt32("Unk1");
                    readInt32("Unk2");
                }
            }
            catch { }
        }
    }
}
