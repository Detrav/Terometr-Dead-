using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Detrav.TeraApi.Data;
using Detrav.Sniffer.Tera;
using Detrav.TeraApi.Events;

namespace Detrav.TeraPluginsManager.Core
{
    partial class TeraConnection
    {
        ulong selfId;
        Dictionary<ulong, TeraPlayer> players = new Dictionary<ulong, TeraPlayer>();
        Dictionary<ulong, TeraProjectile> projectiles = new Dictionary<ulong, TeraProjectile>();

        TeraPlayer login(TeraPacketParser p)
        {
            string n = (string)p["name"].value;
            ulong i = (ulong)p["player id"].value;
            ushort l = (ushort)p["level"].value;
            TeraPlayer player;
            if (players.TryGetValue(i, out player)) { player.id = i; player.name = n; player.level = l; }
            else
            {
                player = new TeraPlayer() { name = n, id = i, level = l };
                players.Add(i, player);
            }
            selfId = i;
            return player;
        }
        
        TeraPlayer spawnPlayer(TeraPacketParser p)
        {
            string n = (string)p["name"].value;
            ulong i = (ulong)p["id"].value;
            ushort l = 0;
            TeraPlayer player;
            if (players.TryGetValue(i, out player)) { player.id = i; player.name = n; player.level = l; }
            else
            {
                player = new TeraPlayer() { name = n, id = i, level = l };
                players.Add(i, player);
            }
            return player;
        }

        TeraPlayer deSpawnPlayer(TeraPacketParser p)
        {
            TeraPlayer pl;
            if (!players.TryGetValue((ulong)p["id"].value, out pl))
                return new TeraPlayer() { id = (ulong)p["id"].value };
            players.Remove(pl.id);
            return pl;
        }

        private OnDamageEventArgs damage(TeraPacketParser p)
        {
            ulong i = (ulong)p["attacket id"].value;
            TeraProjectile pr;
            if(projectiles.TryGetValue(i,out pr))
            {
                return new OnDamageEventArgs(pr,
                    (ulong)p["damage"].value,
                    (ushort)p["type"].value);
            }
            TeraPlayer pl;
            if(players.TryGetValue(i,out pl))
            {
                return new OnDamageEventArgs(pl,
                    (ulong)p["damage"].value,
                    (ushort)p["type"].value);
            }
            return null;
        }

        private void deSpawnProjectile(TeraPacketParser p)
        {
            ulong i = (ulong)p["id"].value;
            TeraProjectile pr;
            if (projectiles.TryGetValue(i, out pr))
            {
                projectiles.Remove(i);
            }
        }

        private void spawnProjectile(TeraPacketParser p)
        {
            ulong i = (ulong)p["id"].value;
            ulong pId = (ulong)p["player id"].value;
            TeraProjectile pr;
            TeraPlayer pl;
            if(projectiles.TryGetValue(i,out pr))
            {
                if(players.TryGetValue(pId,out pl))
                {
                    pr.player = pl;
                    return;
                }
                projectiles.Remove(i);
                return;
            }
            if (players.TryGetValue(pId, out pl))
            {
                pr = new TeraProjectile() { id = i, player = pl };
                projectiles.Add(i,pr);
                return;
            }

        }
    }
}