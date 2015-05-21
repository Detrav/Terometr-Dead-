using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi.Events
{
    public class LoginEventArgs : EventArgs
    {
        public string name;
        public ulong id;
        public ushort level;

        public LoginEventArgs(string name, ulong id, ushort level)
        {
            this.name = name;
            this.id = id;
            this.level = level;
        }
    }
}
