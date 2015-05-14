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

namespace Terometr
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

        Sniffer.Capture sniffer;
        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            sniffer = new Sniffer.Capture();
            sniffer.getDevices();
            sniffer.serverIp = "91.225.237.8";
            sniffer.onParsePacket += sniffer_onParsePacket;
            sniffer.start(0);
            dataGrid.Items.Add(new string[3] { i.ToString(), "player name", "player id" });
            i++;
        }
        int i = 0;
        void sniffer_onParsePacket(Sniffer.Connection connection, Sniffer.Tera.TeraPacket packet)
        {
            if(packet.opCode == (ushort)Sniffer.Tera.OpCode2805.S_SPAWN_USER)
            {
                var p =Sniffer.Tera.TeraPacketCreator.create(packet);
                dataGrid.Dispatcher.Invoke(new Action<Sniffer.Tera.TeraPacketParser>((pac) =>
                {
                    dataGrid.Items.Add(new string[3] { i.ToString(), pac["name"].value.ToString(), pac["player id"].value.ToString() });
                }), p);
                i++;
            }
        }

        private void dataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sniffer.stop();
        }
    }
}
