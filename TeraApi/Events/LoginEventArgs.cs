using Detrav.TeraApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi.Events
{
    public class LoginEventArgs : EventArgs
    {
        public TeraPlayer player;

        public LoginEventArgs(TeraPlayer p)
        {
            player = p;
        }
    }
}
