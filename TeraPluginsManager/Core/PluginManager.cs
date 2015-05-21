using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraPluginsManager.Core
{
    class PluginManager
    {
        Assembly[] assemblies;
        Type[] types;
        public PluginManager()
        {
            if (!Directory.Exists("plugins")) Directory.CreateDirectory("plugins");
            var files = Directory.GetFiles("plugins", "*.dll");
            List<Assembly> l = new List<Assembly>();
            List<Type> t = new List<Type>();
            foreach(var file in files)
            {
                Assembly a = Assembly.LoadFrom(file);
                l.Add(a);
                foreach(var v in a.GetTypes())
                {
                    if (v.Name == "Plugin")
                    {
                        t.Add(v);
                        v.GetMethod("register").Invoke(null, null);
                        break;
                    }
                }
                
            }
            assemblies = l.ToArray();
            types = t.ToArray();
        }

        public Type[] getTypes()
        {
            return (Type[])types.Clone();
        }
    }
}
