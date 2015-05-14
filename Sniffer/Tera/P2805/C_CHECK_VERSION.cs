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
                int count = readUInt16(4,"count");
                for (int i = 0; i < count; i++)
                {
                    readInt32((ushort)(6 + 12 * i), "NowShift & NextShift");// 4 + 12
                    readInt32((ushort)(10 + 12 * i),"Unk1");
                    readInt32((ushort)(14 + 12 * i),"Unk2");
                }
            }
            catch { }
        }
    }
}
