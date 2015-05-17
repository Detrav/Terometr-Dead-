using Detrav.Sniffer;
using Detrav.Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.TeraApi
{
    class MySnifferForTest
    {
        Capture sniffer;

        public MySnifferForTest()
        {
            sniffer = new Capture("91.225.237.8");
            sniffer.getDevices();
            sniffer.onParsePacket += sniffer_onParsePacket;
            sniffer.start(0);
        }

        void sniffer_onParsePacket(Connection connection, Detrav.Sniffer.Tera.TeraPacket packet)
        {
            OpCode2805 code = (OpCode2805)packet.opCode;
            TeraPacketParser p;
            switch (code)
            {
                case OpCode2805.S_LOGIN:
                    p = TeraPacketCreator.create(packet);
                    Repository.Instance.updateSelfPlayer((ulong)p["player id"].value, (string)p["name"].value);
                    break;
                case OpCode2805.S_SPAWN_USER:
                    p = TeraPacketCreator.create(packet);
                    Repository.Instance.addOrUpdatePlayer((ulong)p["id"].value, (string)p["name"].value);
                    break;
                case OpCode2805.S_DESPAWN_USER:
                    p = TeraPacketCreator.create(packet);
                    Repository.Instance.removePlayer((ulong)p["id"].value);
                    break;
                case OpCode2805.S_SPAWN_PROJECTILE:
                    p = TeraPacketCreator.create(packet);
                    Repository.Instance.addOrUpdateShot((ulong)p["id"].value, (ulong)p["player id"].value);
                    break;
                case OpCode2805.S_DESPAWN_PROJECTILE:
                    p = TeraPacketCreator.create(packet);
                    Repository.Instance.removeShot((ulong)p["id"].value);
                    break;
                case OpCode2805.S_EACH_SKILL_RESULT:
                    p = TeraPacketCreator.create(packet);
                    Repository.Instance.damage((ulong)p["attacker id"].value, (ushort)p["type"].value, (uint)p["damage"].value);
                    break;
            }
        }

        public void close()
        {
            sniffer.stop();
        }
    }
}
