using Detrav.Sniffer;
using Detrav.Sniffer.Tera;
using Detrav.Terometr.TeraApi.Data;
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
        public MainWindow()
        {
            InitializeComponent();
            listBoxWhatNow.Items.Add("Ожидание запуска драйвера...");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            capture = new Capture();
            InitWindow initWindow = new InitWindow(capture.devices);
            if (initWindow.ShowDialog() != true) { Close(); return; }
            List<string> strs = new List<string>();
            foreach (var el in servers)
                strs.Add(el.serverIp);
            capture.serverIps = strs.ToArray();
            capture.onStartedSniffer += capture_onStartedSniffer;
            capture.onNewConnection += capture_onNewConnection;
            capture.start(initWindow.selectedIndex);
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
                if (el.serverIp == e.connection.srcIp) { serverName = el.serverName; break; }
                else if (el.serverIp == e.connection.dstIp) { serverName = el.serverName; break; }
            SnifferPage snifferPage = new SnifferPage();
            tabControl.Items.Add(new TabItem() { Header = serverName, Content = snifferPage });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            capture.Dispose();
        }

    }
}
