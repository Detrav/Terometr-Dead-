using Detrav.TeraApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Detrav.TeraApi.Events
{
    public class SpawnPlayerEventArgs: EventArgs
    {
        public TeraPlayer player;

        public SpawnPlayerEventArgs(TeraPlayer p)
        {
            player = p;
        }
    }
}