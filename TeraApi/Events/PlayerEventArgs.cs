using Detrav.TeraApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Detrav.TeraApi.Events
{
    public class PlayerEventArgs: EventArgs
    {
        public TeraPlayer player;

        public PlayerEventArgs(TeraPlayer p)
        {
            player = p;
        }
    }
}