using Detrav.Sniffer;
using Detrav.Terometr.Core.Data;
using Detrav.Terometr.UserElements;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Detrav.Terometr.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Core.TeraApi teraApi;
        DispatcherTimer timer;
        Dictionary<Connection,List<IUserElement>> connections = new Dictionary<Connection,List<IUserElement>>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonClose_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Close();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            menuButton_ContextMenuOpening(sender, null);
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void menuButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            for(int i = 0; i<tabControl.Items.Count;i++)
            {
                var mm = new MenuItem();
                mm.Header = (tabControl.Items[i] as TabItem).Header;
                mm.Click += (a,b) => {
                    foreach (TabItem t in tabControl.Items)
                        if (t.Header == (a as MenuItem).Header)
                        {
                            tabControl.SelectedItem = t;
                            break;
                        }
                };
                menu.Items.Add(mm);
            }
            (sender as Button).ContextMenu = menu;
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                tabControl.SelectedIndex = tabControl.Items.Count - 1;
            else
                tabControl.SelectedIndex--;
        }

        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == tabControl.Items.Count - 1)
                tabControl.SelectedIndex = 0;
            else
                tabControl.SelectedIndex++;
        }

        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not released yet");
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not released yet");
        }

        private void buttonVolume_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not released yet");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = Properties.Settings.Default.windowHeight;
            this.Top = Properties.Settings.Default.windowTop;
            this.Left = Properties.Settings.Default.windowLeft;
            this.Width = Properties.Settings.Default.windowWidth;
            ServerInfoItem[] servers = ServerInfoItem.servers();
            var capture = new Detrav.Sniffer.Capture();
            //Запускаем окно настроек соединения
            InitWindow initWindow = new InitWindow(capture.devices, ServerInfoItem.getServersName(servers));
            if (initWindow.ShowDialog() != true) { Close(); return; }
            capture.serverIps = new string[1] { servers[initWindow.selectedIndexServer].serverIp };
            teraApi = new Core.TeraApi(capture);
            //Таймер и синхронизированные события:
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(101);
            timer.Tick += timer_Tick;
            teraApi.onStartSnifferSync += teraApi_onStartSnifferSync;
            teraApi.onEndConnectionSync += teraApi_onEndConnectionSync;
            teraApi.onNewConnectionSync += teraApi_onNewConnectionSync;
            teraApi.start(initWindow.selectedIndexDevice);
            timer.Start();
        }

        void teraApi_onNewConnectionSync(object sender, Sniffer.ConnectionEventArgs e)
        {
            connections.Add(e.connection,new List<IUserElement>());
            reMakeTabs();
        }

        void teraApi_onEndConnectionSync(object sender, Sniffer.ConnectionEventArgs e)
        {
            connections.Remove(e.connection);
            reMakeTabs();
        }

        void teraApi_onStartSnifferSync(object sender, EventArgs e)
        {
            labelInfo.Content = "Драйвер готов, ожидаем подключений...";
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            teraApi.doEvents();
            timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try { teraApi.stop(); }
            catch { }
            Properties.Settings.Default.windowHeight = this.Height;
            Properties.Settings.Default.windowTop = this.Top;
            Properties.Settings.Default.windowLeft = this.Left;
            Properties.Settings.Default.windowWidth = this.Width;
            Properties.Settings.Default.Save();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            reMakeTabs();
        }

        private void reMakeTabs()
        {
            //if(checkBoxDpsMeter)
        }
    }
}
