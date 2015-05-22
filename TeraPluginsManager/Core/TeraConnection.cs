using Detrav.Sniffer;
using Detrav.Sniffer.Tera;
using Detrav.TeraApi;
using Detrav.TeraApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraPluginsManager.Core
{
    partial class TeraConnection : ITeraConnection
    {
        public event OnLogin onLogin;
        public event OnTick onTick;
        public event OnSpawnPlayer onSpawnPlayer;
        public event OnDeSpawnPlayer onDeSpawnPlayer;
        public event OnDamage onDamage;

        IPlugin[] plugins;
        public TeraConnection(Type[] types)
        {
            plugins = new IPlugin[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                plugins[i] = Activator.CreateInstance(types[i]) as IPlugin;
            }
            version = TeraPacketCreator.getVersion();
        }

        public void doEvent()
        {
            if (onTick != null)
                onTick(this, EventArgs.Empty);
        }

        public void unLoad()
        {
            foreach (var p in plugins)
                p.unLoad();
        }

        public void load()
        {
            foreach (var p in plugins)
                p.load(this);
        }

        OpCodeVersion version;

        public void parsePacket(object sender, EventArgs e)
        {
            PacketEventArgs ev = (PacketEventArgs)e;
            switch (version)
            {
                case OpCodeVersion.P2805:
                    switch ((OpCode2805)ev.packet.opCode)
                    {
                        case OpCode2805.S_LOGIN:
                            {
                                TeraPacketParser p = TeraPacketCreator.create(ev.packet);
                                var pl = login(p);
                                if (onLogin != null) onLogin(this, new PlayerEventArgs(pl));
                            }
                            break;
                        case OpCode2805.S_SPAWN_USER:
                            {
                                TeraPacketParser p = TeraPacketCreator.create(ev.packet);
                                var pl = spawnPlayer(p);
                                if (onSpawnPlayer != null) onSpawnPlayer(this, new PlayerEventArgs(pl));
                            }
                            break;
                        case OpCode2805.S_DESPAWN_USER:
                            {
                                TeraPacketParser p = TeraPacketCreator.create(ev.packet);
                                var pl = deSpawnPlayer(p);
                                if (onDeSpawnPlayer != null) onDeSpawnPlayer(this, new PlayerEventArgs(pl));
                            }
                            break;
                        case OpCode2805.S_SPAWN_PROJECTILE:
                            {
                                TeraPacketParser p = TeraPacketCreator.create(ev.packet);
                                spawnProjectile(p);
                            }
                            break;
                        case OpCode2805.S_DESPAWN_PROJECTILE:
                            {
                                TeraPacketParser p = TeraPacketCreator.create(ev.packet);
                                deSpawnProjectile(p);
                            }
                            break;
                        case OpCode2805.S_EACH_SKILL_RESULT:
                            {
                                TeraPacketParser p = TeraPacketCreator.create(ev.packet);
                                var el = damage(p);
                                if (el != null)
                                    if (onDamage != null) onDamage(this, el);
                            }
                            break;
                    }
                    break;
            }
        }
    }
}
