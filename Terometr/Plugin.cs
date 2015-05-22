using Detrav.TeraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr
{
    class Plugin : IPlugin
    {

        public static void register()
        {
            //System.Windows.MessageBox.Show("Registered!");
        }
        MainWindow w;
        ITeraConnection parent;
        public void load(ITeraConnection parent)
        {
            this.parent = parent;
            parent.onLogin += parent_onLogin;
            w = new MainWindow();
            show();
        }

        void parent_onLogin(object sender, Detrav.TeraApi.Events.PlayerEventArgs e)
        {
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
    }
}
