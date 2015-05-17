using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.TeraApi.Data
{
    struct DpsInfo
    {
        public double dps;
        public string name;
        public double damage;

        public DpsInfo Copy()
        {
            return new DpsInfo { dps = this.dps,name = this.name,damage = this.damage };
        }
    }
}
