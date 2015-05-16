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
            opCodes2805.Add((ushort)OpCode2805.S_LOGIN, typeof(P2805.S_LOGIN));
            opCodes2805.Add((ushort)OpCode2805.S_SPAWN_USER, typeof(P2805.S_SPAWN_USER));
            opCodes2805.Add((ushort)OpCode2805.S_DESPAWN_PROJECTILE, typeof(P2805.S_DESPAWN_PROJECTILE));

            opCodes2805.Add((ushort)OpCode2805.S_SPAWN_PROJECTILE, typeof(P2805.S_SPAWN_PROJECTILE));
            opCodes2805.Add((ushort)OpCode2805.S_DESPAWN_USER, typeof(P2805.S_DESPAWN_USER));
          /* 
           * На вскидку след пакеты нужны полюбому
           * 21055 S_CREATURE_CHANGE_HP
           * 21742 S_ACTION_END
           * 39998 S_SPAWN_PROJECTILE
           *+48244 S_LOGIN
           * 55589 S_EACH_SKILL_RESULT
           * 57037 S_NPC_LOCATION
           * 57105 S_DESPAWN_PROJECTILE
           * 62393 S_ACTION_STAGE
           * 62584 S_SPAWN_USER
           */
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
