using Detrav.TeraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr
{
    public class Plugin : IPlugin
    {

        public static void register()
        {
            //System.Windows.MessageBox.Show("Registered!");
        }
        MainWindow w;
        ITeraConnection parent;
        Dictionary<ulong, Player> players = new Dictionary<ulong,Player>();
        ulong selfId = 0;
        public void load(ITeraConnection parent)
        {
            this.parent = parent;
            parent.onLogin += parent_onLogin;
            parent.onDamage += parent_onDamage;
            parent.onTick += parent_onTick;
            w = new MainWindow(this);
            show();
        }

        void parent_onTick(object sender, EventArgs e)
        {
            SortedList<double,Player> list = new SortedList<double,Player>(new DuplicateKeyComparer<double>());
            double dpsMax = 0;
            foreach(var p in players)
            {
                p.Value.tick();
                if (p.Value.dps > 0)
                {
                    dpsMax = Math.Max(p.Value.dps,dpsMax);
                    list.Add(p.Value.dps, p.Value);
                }
            }
            w.updateDpsList(list, dpsMax, selfId);
        }

        void parent_onDamage(object sender, TeraApi.Events.DamageEventArgs e)
        {
            if (e.type == 1)
            {
                if (e.player.inParty)
                {
                    Player p;
                    if (!players.TryGetValue(e.player.id, out p))
                    {
                        p = new Player(e.player.id, e.player.name);
                        players.Add(p.id, p);
                    }
                    p.dmg(e.damage);
                }
            }
        }

        void parent_onLogin(object sender, Detrav.TeraApi.Events.PlayerEventArgs e)
        {
            selfId = e.player.id;
            w.changeTitle(e.player.name);
        }

        public void show()
        {
            w.Show();
            //w.WindowState = System.Windows.WindowState.Normal;
        }

        public void unLoad()
        {
            w.close = true;
            w.Close();
        }


        public void hide()
        {
            w.Hide();
            //w.WindowState = System.Windows.WindowState.Minimized;
        }

        public void clear()
        {
            players.Clear();
        }
    }

    /// <summary>
    /// Comparer for comparing two keys, handling equality as beeing greater
    /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DuplicateKeyComparer<TKey>
                    :
                 IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = y.CompareTo(x);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }

        #endregion
    }
}
