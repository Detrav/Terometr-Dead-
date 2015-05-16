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
                readUInt32(4,"visualeffect+count");//4 
                readUInt64(8,"attacker id");//8
                readUInt64(16,"target id");//16
                readUInt32(24,"creature model id");//24
                readUInt32(28,"skill id");//28
                //readUInt32("shift 1");//32
                readUInt64(36,"attak id");//36
                //readUInt32("shift 2");//44
                readUInt32(48,"damage");//48
                readUInt16(52,"type");//52
                readByte(54,"crit");//54
                readByte(55,"електровсплеск");//55
                readByte(56,"overturned 1");//56 //Походу крит
                readByte(57,"overturned 2");//57
            }
            catch { }
        }
    }
}
