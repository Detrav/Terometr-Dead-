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
        //public bool inBattle;
        public double dps { get { return damage / span.TotalSeconds; } }
        public TimeSpan span { get { return stop - start; } }
        public DateTime start = DateTime.MinValue;
        public DateTime stop = DateTime.MinValue;
        public DateTime last = DateTime.MinValue;
        public TimeSpan timeout = TimeSpan.FromMilliseconds(3.14);

        public Player(ulong id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public void dmg(double damage)
        {
            if (DateTime.Now - last > timeout)
            {
                start += DateTime.Now - last;
            }
            this.damage += damage;
            last = DateTime.Now;
        }

        internal void tick()
        {
            if (DateTime.Now - last < timeout)
                stop = DateTime.Now;
        }
    }
}
