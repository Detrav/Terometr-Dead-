using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi
{
    public interface IPlugin
    {
        public void register(ITera parent);
        public void show();
        public void unRegister();
    }
}
