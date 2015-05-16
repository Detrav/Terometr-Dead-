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
using Terometr.Data;
using Terometr.Themes;

namespace Terometr.Windows
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

        MySnifferForTest test = new MySnifferForTest();
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            test.close();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Repository.Instance.updateWPFDpss(listBox.Items);
        }
    }
}
