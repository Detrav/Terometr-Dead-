using Detrav.TeraApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi.Events
{
    public class DamageEventArgs : EventArgs
    {
        public TeraPlayer player;
        public TeraProjectile projectile;
        public ulong damage;
        public ushort type;

        public DamageEventArgs(TeraPlayer p,ulong dmg,ushort t)
        {
            player = p;
            damage = dmg;
            type = t;
        }

        public DamageEventArgs(TeraProjectile p, ulong dmg, ushort t)
        {
            player = p.player;
            projectile = p;
            damage = dmg;
            type = t;
        }
    }
}
