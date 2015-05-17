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
            Properties.Settings.Default.adapterIndex++;
            Properties.Settings.Default.Save();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
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

        Detrav.Terometr.TeraApi.MySnifferForTest test = new Detrav.Terometr.TeraApi.MySnifferForTest();
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            test.close();
            myNotifyIcon.Visibility = System.Windows.Visibility.Hidden;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Detrav.Terometr.TeraApi.Repository.Instance.updateWPFDpss(listBox.Items);
        }
    }
}
