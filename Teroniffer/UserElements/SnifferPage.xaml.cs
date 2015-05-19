using Detrav.Sniffer.Tera;
using Microsoft.Win32;
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
using System.Windows.Threading;
using Teroniffer.Core;

namespace Teroniffer.UserElements
{
    /// <summary>
    /// Логика взаимодействия для SnifferPage.xaml
    /// </summary>
    public partial class SnifferPage : UserControl
    {
        List<DataPacket> packets = new List<DataPacket>();
        DispatcherTimer timerStatusBar;
        /*internal IEnumerable dataPackets { get { return (IEnumerable)GetValue(dataPacketsProperty); } set { SetValue(dataPacketsProperty, value); } }
        public static readonly DependencyProperty dataPacketsProperty = DependencyProperty.Register("dataPackets", typeof(IEnumerable), typeof(SnifferPage), null);*/
        
        public SnifferPage()
        {
            InitializeComponent();
            timerStatusBar = new DispatcherTimer();
            timerStatusBar.Interval = TimeSpan.FromMilliseconds(101);
            timerStatusBar.Tick += timerStatusBar_Tick;
            timerStatusBar.Start();
        }

        void timerStatusBar_Tick(object sender, EventArgs e)
        {
            timerStatusBar.Stop();
            int count = 0;
            lock(packets)
            {
                count = packets.Count;
            }
            labelPacketCount.Content = count.ToString();
            double m = GC.GetTotalMemory(true);
            int order = 0;
            while (m >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                m = m / 1024;
            }

            labelMemoryUsage.Content = String.Format("{0:0.##} {1}", m, sizes[order]);
            timerStatusBar.Start();

        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            lock (packets)
            {
                packets.Clear();
            }
            buttonRefresh_Click(sender, e);
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text file (*.txt)|*.txt";
            if (sfd.ShowDialog()==true)
            {
                using (System.IO.TextWriter tw = new System.IO.StreamWriter(sfd.OpenFile()))
                {
                    lock (packets)
                    {
                        foreach (var p in packets)
                            tw.WriteLine(p.getTeraPacket().ToString());
                    }
                }
            }
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonWhite_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                listBoxWhite.Items.Add((dataGrid.SelectedItem as DataPacket).opCode);
                tabControl.SelectedIndex = 1;
            }
        }

        private void buttonBlack_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                listBoxBlack.Items.Add((dataGrid.SelectedItem as DataPacket).opCode);
                tabControl.SelectedIndex = 1;
            }
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {

            int take = 1000;
            Int32.TryParse(textBoxCount.Text, out take);
            int skip = 0;
            Int32.TryParse(textBoxSkip.Text, out skip);
            textBoxSkip.Text = (skip - take).ToString();
            buttonRefresh_Click(this, e);
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            int skip = 0;
            Int32.TryParse(textBoxSkip.Text,out skip);
            int take = 1000;
            Int32.TryParse(textBoxCount.Text,out take);
            lock (packets)
            {
                var packs = packets.Select(p => p);
                //Проверка на тип пакета
                if (comboBoxType.SelectedIndex == 1)
                    packs = packs.Where(p => p.type == Detrav.Sniffer.Tera.TeraPacket.Type.Recv);
                else if (comboBoxType.SelectedIndex == 2)
                    packs = packs.Where(p => p.type == Detrav.Sniffer.Tera.TeraPacket.Type.Send);
                //Проверка на белый лист
                List<object> ws = new List<object>();
                if(listBoxWhite.Items.Count>0)
                {
                    
                    foreach (var item in listBoxWhite.Items)
                        ws.Add(item);
                    packs = from p in packs where ws.Contains(p.opCode) select p;
                }
                //Проверка на черный лист
                List<object> bs = new List<object>();
                if (listBoxBlack.Items.Count > 0)
                {
                    
                    foreach (var item in listBoxBlack.Items)
                        bs.Add(item);
                    packs = from p in packs where !bs.Contains(p.opCode) select p;
                }
                //Поиск будет отдельно
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = packs.Skip(skip).Take(take);
                dataGrid.Items.Refresh();    
            }
            
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            int take = 1000;
            Int32.TryParse(textBoxCount.Text, out take);
            int skip = 0;
            Int32.TryParse(textBoxSkip.Text, out skip);
            textBoxSkip.Text = (skip + take).ToString();
            buttonRefresh_Click(this, e);
        }

        private void buttonFilterImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonFilterExport_Click(object sender, RoutedEventArgs e)
        {

        }

        static string[] sizes = { "B", "KB", "MB", "GB" };
        internal void parsePacket(Detrav.Sniffer.Tera.TeraPacket teraPacket)
        {
            lock (packets)
            {
                packets.Add(new DataPacket(packets.Count, teraPacket));
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dataGrid.SelectedItem!=null)
            textBlockPacket.Text = Detrav.Sniffer.Tera.TeraPacketCreator.create((dataGrid.SelectedItem as DataPacket).getTeraPacket()).ToString();
            /*richTextBox.Document.Blocks.Clear();
            richTextBox.Selection.Text = Detrav.Sniffer.Tera.TeraPacketCreator.create((dataGrid.SelectedItem as DataPacket).getTeraPacket()).ToString();*/
        }
    }
}
