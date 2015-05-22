using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr
{
    class Player
    {
        public ulong id;
        public string name;
        public double damage;
        public bool inBattle;
        public double dps { get { return damage / span.TotalSeconds; } }
        public TimeSpan span { get { return stop - start; } }
        public DateTime start = DateTime.MinValue;
        public DateTime stop = DateTime.MinValue;

        public Player(ulong id, string name)
        {
            this.id = id;
            this.name = name;
            inBattle = false;
        }

        public void startBattle()
        {
            if (!inBattle)
            {
                start += DateTime.Now - stop;
                inBattle = true;
            }
        }

        public void stopBattle()
        {
            if (inBattle)
            {
                stop = DateTime.Now;
                inBattle = false;
            }
        }

        public void dmg(double damage)
        {

            this.damage += damage;
        }

        internal void tick()
        {
            if (inBattle)
            {
                stop = DateTime.Now;
            }
        }
    }
}
