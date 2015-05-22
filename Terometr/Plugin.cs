using Detrav.TeraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terometr
{
    class Plugin : IPlugin
    {

        public static void register()
        {
            //System.Windows.MessageBox.Show("Registered!");
        }
        MainWindow w;
        public void load(ITeraConnection parent)
        {
            w = new MainWindow();
            show();
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
