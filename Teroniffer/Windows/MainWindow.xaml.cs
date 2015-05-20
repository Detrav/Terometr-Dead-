using Detrav.Sniffer;
using Detrav.Sniffer.Tera;
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
using Teroniffer.Core;
using Teroniffer.UserElements;

namespace Teroniffer.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Capture capture;
        ServerInfoItem[] servers = ServerInfoItem.servers();
        Dictionary<Connection, SnifferPage> snifferPages;
        public MainWindow()
        {
            InitializeComponent();
            snifferPages = new Dictionary<Connection, SnifferPage>();
            listBoxWhatNow.Items.Add("Ожидание запуска драйвера...");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            capture = new Capture();
            //Запускаем окно настроек соединения
            InitWindow initWindow = new InitWindow(capture.devices,ServerInfoItem.getServersName(servers));
            if (initWindow.ShowDialog() != true) { Close(); return; }
            capture.serverIps = new string[1]{ servers[initWindow.selectedIndexServer].serverIp };
            capture.onStartedSniffer += capture_onStartedSniffer;
            capture.onNewConnection += capture_onNewConnection;
            capture.onParsePacket += capture_onParsePacket;
            capture.start(initWindow.selectedIndexDevice);
            test();
        }

        void capture_onParsePacket(object sender, PacketEventArgs e)
        {
            SnifferPage sn;
            lock(snifferPages)
            {
                snifferPages.TryGetValue(e.connection, out sn);
            }
            sn.parsePacket(e.packet);
        }

        void capture_onStartedSniffer(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                listBoxWhatNow.Items.Add("Ожидание соединений...");
            }));
        }
            

        void capture_onNewConnection(object sender, ConnectionEventArgs e)
        {
            string serverName = "Unknown";
            foreach(var el in servers)
                if (el.serverIp == e.connection.srcIp) { serverName = e.connection.dstPort.ToString(); break; }
                else if (el.serverIp == e.connection.dstIp) { serverName = e.connection.srcPort.ToString(); break; }
            Dispatcher.Invoke(new Action<string>((sName) =>
            {
                SnifferPage snifferPage = new SnifferPage();
                tabControl.Items.Add(new TabItem() { Header = sName, Content = snifferPage });
                lock(snifferPages)
                {
                    snifferPages.Add(e.connection,snifferPage);
                }
            }),serverName);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            capture.Dispose();
        }


        private void test()
        {
            Connection c = new Connection("", 0, "", 0);
            capture_onNewConnection(this, new ConnectionEventArgs(c));

            Random r = new Random();

            for(int i =0; i<1000;i++)
            {
                byte[] bb = new byte[r.Next(100,200)];
                r.NextBytes(bb);
                if(r.Next(0,2)==0) capture_onParsePacket(this, new PacketEventArgs(c,new TeraPacket(bb, TeraPacket.Type.Recv)));
                else capture_onParsePacket(this, new PacketEventArgs(c,new TeraPacket(bb, TeraPacket.Type.Send)));
            }
        }
    }
}
