using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer.Tera.P2805
{
    /// <summary>
    /// Показывает текущий статус игрока, в бою или нет
    /// </summary>
    class S_USER_STATUS : TeraPacketParser
    {
        public S_USER_STATUS(TeraPacket packet)
            : base(packet)
        {
            try
            {
                readUInt64(4, "id");
                readByte(12, "status");
            }
            catch { }
        }
    }
}
