using Detrav.Sniffer;
using Detrav.TeraApi;
using Detrav.TeraPluginsManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Detrav.TeraPluginsManager.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Tera tera;
        DispatcherTimer timer;
        Dictionary<Connection, ITeraConnection> teraConnections = new Dictionary<Connection,ITeraConnection>();
        PluginManager plaginManager = new PluginManager();
        Dictionary<Connection, Button> buttons = new Dictionary<Connection, Button>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Capture c = new Capture();
            ServerInfoItem[] servers = ServerInfoItem.servers();
            InitWindow initWindow = new InitWindow(c.devices, ServerInfoItem.getServersName(servers));
            if (initWindow.ShowDialog() != true) { Close(); return; }
            c.serverIps = new string[1] { servers[initWindow.selectedIndexServer].serverIp };
            tera = new Tera(c);
            tera.onStartSnifferSync += tera_onStartSnifferSync;
            tera.onNewConnectionSync += tera_onNewConnectionSync;
            tera.onEndConnectionSync += tera_onEndConnectionSync;
            tera.onParsePacketSync += tera_onParsePacketSync;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(101);
            timer.Tick += timer_Tick;
            timer.Start();

            tera.start(initWindow.selectedIndexDevice);
        }

        void tera_onParsePacketSync(object sender, PacketEventArgs e)
        {
            ITeraConnection con;
            if (teraConnections.TryGetValue(e.connection, out con))
            {
                con.parsePacket(this,e);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            tera.doEvents();
            foreach (var t in teraConnections)
                t.Value.doEvent();
            timer.Start();
        }

        void tera_onEndConnectionSync(object sender, ConnectionEventArgs e)
        {
            ITeraConnection con;
            if(teraConnections.TryGetValue(e.connection, out con))
            {
                con.unLoad();
                teraConnections.Remove(e.connection);
            }
            Button b;
            if(buttons.TryGetValue(e.connection,out b))
            {
                stackPanel.Children.Remove(b);
                buttons.Remove(e.connection);
            }
        }

        

        void tera_onNewConnectionSync(object sender, ConnectionEventArgs e)
        {
            Button b = new Button();
            b.Content = e.connection;
            b.Click += b_Click;
            stackPanel.Children.Add(b);
            buttons.Add(e.connection, b);
            TeraConnection t = new TeraConnection(plaginManager.getTypes());
            teraConnections.Add(e.connection,t);
            t.load();
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            var c = (sender as Button).Content as Connection;
            ITeraConnection val;
            teraConnections.TryGetValue(c, out val);
            val.showAll();
        }

        void tera_onStartSnifferSync(object sender, EventArgs e)
        {
            stackPanel.Children.Add(new Label() {Content = "Драйвер готов, ожидаем соединений..." });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var t in teraConnections)
                t.Value.unLoad();
            if (tera != null)
                tera.stop();
        }
    }
}
