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

        public event OnLogin onLogin;

        public event OnTick onTick;

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
            switch(version)
            {
                case OpCodeVersion.P2805:
                    switch((OpCode2805)ev.packet.opCode)
                    {
                        case OpCode2805.S_LOGIN:
                            TeraPacketParser p = TeraPacketCreator.create(ev.packet);
                            if (onLogin != null) onLogin(this, new LoginEventArgs(login(p)));
                            break;
                    }
                    break;
            }
        }
    }
}
