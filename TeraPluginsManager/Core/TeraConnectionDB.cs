using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Detrav.TeraApi.Data;
using Detrav.Sniffer.Tera;

namespace Detrav.TeraPluginsManager.Core
{
    partial class TeraConnection
    {
        ulong selfId;
        Dictionary<ulong, TeraPlayer> players = new Dictionary<ulong, TeraPlayer>();

        TeraPlayer login(TeraPacketParser p)
        {
            string n = (string)p["name"].value;
            ulong i = (ulong)p["player id"].value;
            ushort l = (ushort)p["level"].value;
            TeraPlayer player = new TeraPlayer() { name = n, id = i, level = l };
            if (players.TryGetValue(i, out player)) { player.id = i; player.name = n; player.level = l; }
            else players.Add(i, player);
            selfId = i;
            return player;
        }
        
        TeraPlayer spawnPlayer(TeraPacketParser p)
        {
            string n = (string)p["name"].value;
            ulong i = (ulong)p["id"].value;
            ushort l = 0;
            TeraPlayer player = new TeraPlayer() { name = n, id = i, level = l };
            if (players.TryGetValue(i, out player)) { player.id = i; player.name = n; player.level = l; }
            else players.Add(i, player);
            return player;
        }
    }
}