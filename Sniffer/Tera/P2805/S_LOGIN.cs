using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer.Tera.P2805
{
    class S_LOGIN : TeraPacketParser
    {
        public S_LOGIN(TeraPacket packet)
            : base(packet)
        {
            try
            {
                ushort start_name = readUInt16(4,"start name");//4
                ushort end_name = readUInt16(6,"end name");//6
                readUInt16(8,"visual len");//8
                readUInt32(10,"sex race class");//10
                readUInt32(14,"model");//14
                readUInt64(18,"player id");//18
                readUInt64(26,"unique id");//26
                readUInt16(61,"level");//61
                readString(start_name,"name", end_name);
            }
            catch { }
        }
    }
}
