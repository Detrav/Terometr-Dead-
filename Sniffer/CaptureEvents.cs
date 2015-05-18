using Detrav.Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer
{
    public partial class Capture
    {
        //При получении тера пакета
        public delegate void OnParsePacket(Connection connection, TeraPacket packet);
        public event OnParsePacket onParsePacket;
        /*
         * Новое сеодинение
         * Конец соединения
         * 
         */
    }
}
