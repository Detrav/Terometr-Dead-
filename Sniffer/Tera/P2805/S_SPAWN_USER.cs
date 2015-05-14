using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sniffer.Tera.P2805
{
    class S_SPAWN_USER : TeraPacketParser
    {
        public S_SPAWN_USER(TeraPacket packet)
            : base(packet)
        {
            try
            {
                ushort name_start = readUInt16(8, "name start");
                ushort guild_start = readUInt16(10, "guild start");
                ushort guild_rank_start = readUInt16(12, "guild rank start");
                ushort guild_title_start = readUInt16(20, "guild title start");
                readUInt64(34, "player id");
                //readUInt64(38, "unique id");
                readString(name_start, "name");
                readString(guild_start, "guild");
                readString(guild_rank_start, "guild rank");
                readString(guild_title_start, "guild title");
            }
            catch { }
        }
    }
}