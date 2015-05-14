using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Tera
{
    public class TeraPacketCreator
    {
        private Dictionary<ushort, Type> opCodes2805 = new Dictionary<ushort, Type>();
        private static TeraPacketCreator instance = null;
        private TeraPacketCreator()
        {
            opCodes2805.Add((ushort)OpCode2805.C_CHECK_VERSION, typeof(P2805.C_CHECK_VERSION));
            opCodes2805.Add((ushort)OpCode2805.S_EACH_SKILL_RESULT, typeof(P2805.S_EACH_SKILL_RESULT));
        }

        private static TeraPacketCreator Instance
        {
            get
            {
                if (instance == null)
                    instance = new TeraPacketCreator();
                return instance;
            }
        }

        public static TeraPacketParser create(TeraPacket packet)
        {
            Type p;
            if (Instance.opCodes2805.TryGetValue(packet.opCode,out p))
             return (TeraPacketParser)Activator.CreateInstance(p, packet);
            return new TeraPacketParser(packet);
        }
    }
}
