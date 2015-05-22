using Detrav.TeraApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi.Events
{
    public class OnDamageEventArgs : EventArgs
    {
        public TeraPlayer player;
        public TeraProjectile projectile;
        public ulong damage;
        public ushort type;

        public OnDamageEventArgs(TeraPlayer p,ulong dmg,ushort t)
        {
            player = p;
            damage = dmg;
            type = t;
        }

        public OnDamageEventArgs(TeraProjectile p, ulong dmg, ushort t)
        {
            player = p.player;
            projectile = p;
            damage = dmg;
            type = t;
        }
    }
}
