using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Detrav.Terometr.TeraApi.Data

namespace Detrav.Terometr.TeraApi
{
    //Да да, у него должно быть другое применение, однако я захотел назвать синглтон так
    class Repository
    {
        //Помню както первый раз услышал про синглтон, долго не мог понять с чем его едять
        //про патерны вобще молчу, хоть бы в университете когда учили расказали про них
        //И вот я прихожу на собеседование, и меня спрашивают, какие патерны знаете...... Чё?
        //Сейчас я знаю порядка 10 вариантов синглтона :)
        //Такой вариант предлагают мелкомягкие, хотя я встречал более рациональный, этот мне нравиться
        private static volatile Repository instance;
        private static object syncRoot = new object();

        public static Repository Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Repository();
                    }
                }
                return instance;
            }
        }


        private Repository()
        {
            players = new Dictionary<ulong, PlayerInfo>();
            shots = new Dictionary<ulong, ulong>();
            dpss = new Dictionary<ulong, DpsInfo>();
            selfId = 0;
        }

        Dictionary<ulong, PlayerInfo> players;
        Dictionary<ulong, ulong> shots;
        Dictionary<ulong, DpsInfo> dpss;
        ulong selfId;

        public void addOrUpdatePlayer(ulong playerId, string playerName)
        {
            PlayerInfo p;
            if (!players.TryGetValue(playerId, out p))
            {
                p = new PlayerInfo() { id = playerId, name = playerName };
                players.Add(playerId, p);
            }
            else
            {
                p.name = playerName;
            }
        }

        public void updateSelfPlayer(ulong playerId, string playerName)
        {
            selfId = playerId;
            addOrUpdatePlayer(playerId, playerName);
        }

        public void addOrUpdateShot(ulong shotId, ulong playerId)
        {
            ulong p;
            if (!shots.TryGetValue(shotId, out p))
            {
                shots.Add(shotId, playerId);
            }
            else
            {
                shots[shotId] = playerId;
            }
        }

        public void removeShot(ulong shotId)
        {
            ulong p;
            if (shots.TryGetValue(shotId, out p))
                shots.Remove(shotId);
        }

        public void removePlayer(ulong playerId)
        {
            PlayerInfo p;
            if (players.TryGetValue(playerId, out p))
            {
                players.Remove(playerId);
            }
        }

        public void damage(ulong uId, ushort type, uint value)
        {
            PlayerInfo p;
            if (players.TryGetValue(uId, out p))
            {
                p.addDamage(type, value);
                updateDpss(p);
                return;
            }
            ulong s;
            if (!shots.TryGetValue(uId, out s))
                return;//Error
            if (players.TryGetValue(s, out p))
            {
                p.addDamage(type, value);
                updateDpss(p);
                return;
            }
            //Error
        }

        public void updateDpss(PlayerInfo p)
        {
            lock(dpss)
            {
                DpsInfo dps;
                if(dpss.TryGetValue(p.id,out dps))
                {
                    dps.dps = p.dps;
                    dps.damage = p.damage;
                    dps.update = true;
                }
                else
                {
                    dps = new DpsInfo() { dps = p.dps, damage = p.damage, name =p.name };
                    dpss.Add(p.id, dps);
                    dps.update = true;
                }
            }
        }

        public void updateWPFDpss(System.Windows.Controls.ItemCollection items)
        {
            lock(dpss)
            {
                foreach(var dps in dpss)
                    if(dps.Value.update)
                    {
                        bool flag = true;
                        foreach(Detrav.Terometr.Themes.DpsRow item in items)
                        {
                            if(item.id == dps.Key)
                            {
                                item.playerCount = String.Format("{0}", dps.Value.dps);
                                flag = false;
                                dps.Value.update = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            var dpsRow = new Detrav.Terometr.Themes.DpsRow(
                                dps.Key, 0, dps.Value.name, dps.Value.dps.ToString());
                            items.Add(dpsRow);
                            dps.Value.update = false;
                        }
                    }
            }
        }
    }
}
