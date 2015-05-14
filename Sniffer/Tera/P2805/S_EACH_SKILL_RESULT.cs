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
                readUInt32("visualeffect+count");//4 
                readUInt64("attacker id");//8
                readUInt64("target id");//16
                readUInt32("creature model id");//24
                readUInt32("skill id");//28
                readUInt32("shift 1");//32
                readUInt64("attak id");//36
                readUInt32("shift 2");//44
                readUInt32("damage");//48
                readUInt16("type");//52
                readByte("crit");//54
                readByte("електровсплеск");//55
                readByte("overturned 1");//56
                readByte("overturned 2");//57
            }
            catch { }
        }
    }
}
