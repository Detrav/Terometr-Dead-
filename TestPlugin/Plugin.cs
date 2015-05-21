using Detrav.TeraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    public class Plugin : IPlugin
    {

        public static void register()
        {
            System.Windows.MessageBox.Show("registered");
        }

        public void load(ITeraConnection parent)
        {
            System.Windows.MessageBox.Show("loaded");
            parent.onLogin += parent_onLogin;
        }

        void parent_onLogin(object sender, Detrav.TeraApi.Events.LoginEventArgs e)
        {
            System.Windows.MessageBox.Show(String.Format("new Player detected: {0} {1} {2}",e.level,e.name,e.id));
        }

        public void show()
        {
            throw new NotImplementedException();
        }

        public void unLoad()
        {
            System.Windows.MessageBox.Show("unloaded");
        }
    }
}
