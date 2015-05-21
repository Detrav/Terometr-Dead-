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
        public PluginManager()
        {
            var files = Directory.GetFiles("plugins", "*.dll");
            List<Assembly> l = new List<Assembly>();
            foreach(var file in files)
            {
                l.Add(Assembly.LoadFile(file));
            }
            assemblies = l.ToArray();
        }

        public Type[] getTypes()
        {
            Type[] result = new Type[assemblies.Length];
        }
    }
}
