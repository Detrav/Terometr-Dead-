using Detrav.Sniffer;
using Detrav.Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Detrav.Terometr.TeraApi.Data;
using Detrav.Terometr.Core.Data;

namespace Detrav.Terometr.TeraApi
{
    partial class Repository
    {
        Capture sniffer = null;
        string serverIp = "";
        public string[] devices { get; set; }
        public ServerInfoItem[] serverList
        {
            get
            {
                using (System.IO.TextReader tr = new System.IO.StreamReader("assets/servers.xml"))
                {
                    System.Xml.Serialization.XmlSerializer xer = new System.Xml.Serialization.XmlSerializer(typeof(ServerInfoItem[]));
                    return (ServerInfoItem[])xer.Deserialize(tr);
                }
            }
            set
            {
                using (System.IO.TextWriter tw = new System.IO.StreamWriter("assets/servers.xml"))
                {
                    System.Xml.Serialization.XmlSerializer xer = new System.Xml.Serialization.XmlSerializer(typeof(ServerInfoItem[]));
                    xer.Serialize(tw, value);
                }
            }
        }
        int device;

        public void reStartSniffer(string serv, int dev)
        {
            if (sniffer != null)
            {
                if (serv == serverIp && dev == device)
                    return;
                sniffer.Dispose();
                sniffer = null;
            }
            sniffer = new Capture();
            sniffer.serverIps = new string[] { serv };
            serverIp = serv;
            devices = sniffer.devices;
            sniffer.onParsePacket += sniffer_onParsePacket;
            sniffer.start(dev);
            device = dev;
        }

        void sniffer_onParsePacket(object sender, PacketEventArgs e)
        {
            if (needToClear) clearDpss();//Repository.Instance.needToClear
            OpCode2805 code = (OpCode2805)e.packet.opCode;
            TeraPacketParser p;
            switch (code)
            {
                case OpCode2805.S_LOGIN:
                    p = TeraPacketCreator.create(e.packet);
                    Repository.Instance.updateSelfPlayer((ulong)p["player id"].value, (string)p["name"].value);
                    break;
                case OpCode2805.S_SPAWN_USER:
                    p = TeraPacketCreator.create(e.packet);
                    Repository.Instance.addOrUpdatePlayer((ulong)p["id"].value, (string)p["name"].value);
                    break;
                case OpCode2805.S_DESPAWN_USER:
                    p = TeraPacketCreator.create(e.packet);
                    Repository.Instance.removePlayer((ulong)p["id"].value);
                    break;
                case OpCode2805.S_SPAWN_PROJECTILE:
                    p = TeraPacketCreator.create(e.packet);
                    Repository.Instance.addOrUpdateShot((ulong)p["id"].value, (ulong)p["player id"].value);
                    break;
                case OpCode2805.S_DESPAWN_PROJECTILE:
                    p = TeraPacketCreator.create(e.packet);
                    Repository.Instance.removeShot((ulong)p["id"].value);
                    break;
                case OpCode2805.S_EACH_SKILL_RESULT:
                    p = TeraPacketCreator.create(e.packet);
                    Repository.Instance.damage((ulong)p["attacker id"].value, (ushort)p["type"].value, (uint)p["damage"].value);
                    break;
            }
        }

        public void stopSniffer()
        {
            sniffer.Dispose();
            sniffer = null;
        }
    }
}
