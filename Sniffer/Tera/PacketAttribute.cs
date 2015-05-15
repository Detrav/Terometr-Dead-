using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Tera
{
    [System.AttributeUsage(System.AttributeTargets.All)]
    class PacketAttribute : System.Attribute
    {
        public ushort start;
        public ushort size;
        public byte shift;

        public PacketAttribute(ushort start, ushort size = 1, byte shift = 0)
        {
            this.start = start;
            this.shift = shift;
            this.size = size;
        }
    }
}
