﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Detrav.Terometr.TeraApi.Data;

namespace Detrav.Terometr.TeraApi
{
    //Да да, у него должно быть другое применение, однако я захотел назвать синглтон так
    partial class Repository
    {
        //Помню както первый раз услышал про синглтон, долго не мог понять с чем его едять
        //про патерны вобще молчу, хоть бы в университете когда учили расказали про них
        //И вот я прихожу на собеседование, и меня спрашивают, какие патерны знаете...... Чё?
        //Сейчас я знаю порядка 10 вариантов синглтона :)
        //Такой вариант предлагают мелкомягкие, хотя я встречал более рациональный, но этот мне нравиться больше
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
        int dpsBehavior = 0;
        double battleTimeout = 5;
        public bool needToClear = false;//Repository.instance.sniffer_onParsePacket


        private void addOrUpdatePlayer(ulong playerId, string playerName)
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
        private void updateSelfPlayer(ulong playerId, string playerName)
        {
            selfId = playerId;
            addOrUpdatePlayer(playerId, playerName);
        }
        private void addOrUpdateShot(ulong shotId, ulong playerId)
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
        private void removeShot(ulong shotId)
        {
            ulong p;
            if (shots.TryGetValue(shotId, out p))
                shots.Remove(shotId);
        }
        private void removePlayer(ulong playerId)
        {
            PlayerInfo p;
            if (players.TryGetValue(playerId, out p))
            {
                players.Remove(playerId);
            }
        }
        private void damage(ulong uId, ushort type, uint value)
        {
            PlayerInfo p = null;
            if (players.TryGetValue(uId, out p))
            {
                if(p.addDamage(type, value,dpsBehavior,battleTimeout,selfId))
                    clearDpss();
                updateDpss(p);
                return;
            }
            ulong s;
            if (!shots.TryGetValue(uId, out s))
                return;//Error
            if (players.TryGetValue(s, out p))
            {
                if (p.addDamage(type, value, dpsBehavior, battleTimeout, selfId))
                    clearDpss();
                updateDpss(p);
                return;
            }
            //Error
        }
        private void updateDpss(PlayerInfo p)
        {
            lock(dpss)
            {
                DpsInfo dps;
                if(dpss.TryGetValue(p.id,out dps))
                {
                    dps.dps = p.dps;
                    dps.damage = p.damage;
                }
                else
                {
                    dps = new DpsInfo() { dps = p.dps, damage = p.damage, name =p.name };
                    dpss.Add(p.id, dps);
                }
            }
        }
        private void clearDpss()
        {
            foreach (var p in players)
                p.Value.Clear();
            lock (dpss)
            {
                dpss.Clear();
            }
        }
        internal SortedList<ulong,DpsInfo> updateWPFDpss(out double sumDamage)
        {
            SortedList<ulong, DpsInfo> result = new SortedList<ulong, DpsInfo>();
            double resultDamage = 0;
            lock(dpss)
            {
                foreach(var dps in dpss)
                {
                    result.Add((ulong)dps.Value.damage, dps.Value.Copy());
                    resultDamage += dps.Value.damage;
                }
            }
            sumDamage = resultDamage;
            return result;
        }

        internal void reConfigurate(double _battleTimeout, int _dpsBehavior)
        {
            battleTimeout = _battleTimeout;
            dpsBehavior = _dpsBehavior;
        }
    }
}
