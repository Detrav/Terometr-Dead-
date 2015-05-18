using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer.Tera
{
    [AttributeUsage(AttributeTargets.Property)]
    class PacketInfo : Attribute
    {
        public ushort start;
        public byte shift;

        public PacketInfo(ushort start, byte shift = 0)
        {
            this.start = start;
            this.shift = shift;
        }
    }
}