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

namespace Detrav.Terometr.Windows
{
    /// <summary>
    /// Логика взаимодействия для InitWindow.xaml
    /// </summary>
    public partial class InitWindow : Window
    {
        public int selectedIndexDevice { get; private set; }
        public int selectedIndexServer { get; private set; }
        public InitWindow(string[] listDevices, string[] listServers)
        {
            InitializeComponent();
            foreach (var el in listDevices)
                listBoxDevices.Items.Add(el);
            listBoxDevices.SelectedIndex = Detrav.Terometr.Properties.Settings.Default.initWindowDeviceIndex;
            foreach (var el in listServers)
                listBoxServers.Items.Add(el);
            listBoxServers.SelectedIndex = Detrav.Terometr.Properties.Settings.Default.initWindowServerIndex;
        }

        private void buttonCansel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxDevices.SelectedIndex < 0)
            {
                System.Windows.MessageBox.Show("Нужно выбрать одно из устройств!");
                return;
            }
            if (listBoxServers.SelectedIndex < 0)
            {
                System.Windows.MessageBox.Show("Нужно выбрать один из серверов!");
                return;
            }
            selectedIndexDevice = listBoxDevices.SelectedIndex;
            selectedIndexServer = listBoxServers.SelectedIndex;
            Properties.Settings.Default.initWindowDeviceIndex = listBoxDevices.SelectedIndex;
            Properties.Settings.Default.initWindowServerIndex = listBoxServers.SelectedIndex;
            Properties.Settings.Default.Save();
            DialogResult = true;
        }

        
    }
}
