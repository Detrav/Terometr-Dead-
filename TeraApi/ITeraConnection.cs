using Detrav.TeraApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi
{
    public delegate void OnLogin(object sender, PlayerEventArgs e);
    public delegate void OnTick(object sender, EventArgs e);
    public delegate void OnSpawnPlayer(object sender, PlayerEventArgs e);
    public delegate void OnDeSpawnPlayer(object sender, PlayerEventArgs e);
    public delegate void OnDamage(object sender, OnDamageEventArgs e);

    public interface ITeraConnection
    {
        event OnLogin onLogin;
        event OnSpawnPlayer onSpawnPlayer;
        event OnDeSpawnPlayer onDeSpawnPlayer;
        event OnTick onTick;
        event OnDamage onDamage;

        void doEvent();
        void unLoad();
        void load();

        void parsePacket(object sender, EventArgs e);

        void showAll();
    }
}