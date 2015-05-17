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
using Detrav.Terometr.Themes;
using Detrav.Terometr.TeraApi;

namespace Detrav.Terometr.Windows
{
    /// <summary>
    /// Логика взаимодействия для DpsWindow.xaml
    /// </summary>
    public partial class DpsWindow : Window
    {
        DispatcherTimer timer;
        public DpsWindow()
        {
            InitializeComponent();
            myNotifyIcon.Icon = System.Drawing.Icon.FromHandle(Properties.Resources.icon.GetHicon());
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.IsEnabled = true;
            timer.Tick += timer_Tick;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = Properties.Settings.Default.windowTop;
            this.Left = Properties.Settings.Default.windowLeft;
            this.Height = Properties.Settings.Default.windowHeight;
            this.Width = Properties.Settings.Default.windowWidth;

            Repository.Instance.reStartSniffer(
                Properties.Settings.Default.serverIp,
                Properties.Settings.Default.adapterIndex);
            Repository.Instance.reConfigurate(
                Properties.Settings.Default.battleTimeout,
                Properties.Settings.Default.dpsBehaviorType
                );
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Repository.Instance.stopSniffer();
            myNotifyIcon.Visibility = System.Windows.Visibility.Hidden;
            Properties.Settings.Default.windowTop = this.Top;
            Properties.Settings.Default.windowLeft = this.Left;
            Properties.Settings.Default.windowHeight = this.Height;
            Properties.Settings.Default.windowWidth = this.Width;
            Properties.Settings.Default.Save();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            if(this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MenuItemShow_Click(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }



        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            double damage;
            var array = Repository.Instance.updateWPFDpss(out damage);
            while (listBox.Items.Count < array.Count)
                listBox.Items.Add(new DpsRow());
            while (listBox.Items.Count > array.Count)
                listBox.Items.RemoveAt(0);
            int i = 0;
            foreach(var el in array)
            {
                var dpsRow = (listBox.Items[i] as DpsRow);
                dpsRow.procent = Math.Max(0,Math.Min(el.Value.damage/damage,100));
                dpsRow.playerName = el.Value.name;
                dpsRow.playerCount = String.Format("{0:0.00}", el.Value.dps);
                i++;
            }
            UpdateLayout();
            timer.Start();
        }

        private void buttonConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow window = new ConfigWindow();
            window.ShowDialog();
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            Repository.Instance.needToClear = true;
        }
    }
}
