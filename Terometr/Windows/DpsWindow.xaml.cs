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
using Terometr.Themes;

namespace Terometr.Windows
{
    /// <summary>
    /// Логика взаимодействия для DpsWindow.xaml
    /// </summary>
    public partial class DpsWindow : Window
    {
        public DpsWindow()
        {
            InitializeComponent();
            myNotifyIcon.Icon = System.Drawing.Icon.FromHandle(Properties.Resources.icon.GetHicon());
        }


        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listBox.Items.Add(new DpsRow( 100.0, "Detrav", "1450{40%}" ));
            listBox.Items.Add(new DpsRow( 78.0, "Pofigist", "250{27%}" ));
            listBox.Items.Add(new DpsRow( 40.0, "Varted", "145{10%}" ));
            listBox.Items.Add(new DpsRow(20.0, "Witaly", "14{2%}"));
            listBox.Items.Add(new DpsRow(100.0, "Detrav", "1450{40%}"));
            listBox.Items.Add(new DpsRow(78.0, "Pofigist", "250{27%}"));
            listBox.Items.Add(new DpsRow(40.0, "Varted", "145{10%}"));
            listBox.Items.Add(new DpsRow(20.0, "Witaly", "14{2%}"));
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
    }
}
