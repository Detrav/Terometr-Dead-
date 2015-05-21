using Detrav.TeraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraPluginsManager.Core
{
    class TeraConnection : ITeraConnection
    {
        IPlugin[] plugins;
        public TeraConnection(Type[] types)
        {
            plugins = new IPlugin[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                plugins[i] = Activator.CreateInstance(t) as IPlugin;
            }
        }

        public event OnLogin onLogin;

        public event OnTick onTick;

        public void doEvent()
        {
            throw new NotImplementedException();
        }

        public void unRegister()
        {
            foreach (var p in plugins)
                p.unRegister();
        }

        public void register()
        {
            foreach (var p in plugins)
                p.register(this);
        }
    }
}
