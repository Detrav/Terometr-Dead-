using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Tera.P2805
{
    class S_LOGIN : TeraPacketParser
    {
        public S_LOGIN(TeraPacket packet)
            : base(packet)
        {
            try
            {
                ushort start_name = readUInt16("start name");//4
                ushort end_name = readUInt16("end name");//6
                readUInt16("visual len");//8
                readUInt32("sex race class");//10
                readUInt32("model");//14
                readUInt64("player id");//18
                readUInt64("unique id");//26
                readShift(29);//32 + 29 = 61
                readUInt16("level");//61
                readShift(start_name-61);
                readString("name", end_name);
            }
            catch { }
        }
    }
}
