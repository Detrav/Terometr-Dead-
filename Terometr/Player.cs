using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr
{
    class Player
    {
        ulong id;
        string name;
        double damage;
        double dps { get { return damage / span.TotalSeconds; } }
        TimeSpan span { get { return start - stop; } }
        DateTime start = DateTime.MinValue;
        DateTime stop = DateTime.MinValue;

        public Player(ulong id,string name)
        {
            this.id = id;
            this.name = name;
        }

        public void startBattle()
        {
            start += DateTime.Now - stop;
        }

        public void stopBattle()
        {
            stop = DateTime.Now;
        }

        public void dmg(double damage)
        {
            this.damage += damage;
        }
    }
}
