using System;
using System.Collections;
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
using Teroniffer.Core;

namespace Teroniffer.UserElements
{
    /// <summary>
    /// Логика взаимодействия для SnifferPage.xaml
    /// </summary>
    public partial class SnifferPage : UserControl
    {
        List<DataPacket> packets = new List<DataPacket>();

        /*internal IEnumerable dataPackets { get { return (IEnumerable)GetValue(dataPacketsProperty); } set { SetValue(dataPacketsProperty, value); } }
        public static readonly DependencyProperty dataPacketsProperty = DependencyProperty.Register("dataPackets", typeof(IEnumerable), typeof(SnifferPage), null);*/
        
        public SnifferPage()
        {
            InitializeComponent();
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonWhite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonBlack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            int skip = 0;
            Int32.TryParse(textBoxSkip.Text,out skip);
            int take = 1000;
            Int32.TryParse(textBoxCount.Text,out take);
            lock (packets)
            {
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = packets.Skip(skip).Take(take);
                dataGrid.Items.Refresh();
            }
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonFilterImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonFilterExport_Click(object sender, RoutedEventArgs e)
        {

        }

        internal void parsePacket(Detrav.Sniffer.Tera.TeraPacket teraPacket)
        {
            lock(packets)
            {
                packets.Add(new DataPacket(packets.Count,teraPacket));
            }
        }
    }
}
