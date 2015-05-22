using Detrav.Sniffer.Tera;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;
using Detrav.Teroniffer.Core;
using Detrav.Teroniffer.Windows;

namespace Detrav.Teroniffer.UserElements
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

            if(checkBoxForTimer.IsChecked == true)
            {
                buttonRefresh_Click(sender, new RoutedEventArgs());
            }

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
            if (dataGrid.SelectedItem != null)
            {
                ViewPacketWindow w = new ViewPacketWindow();
                w.setData((dataGrid.SelectedItem as DataPacket).getTeraPacket().data);
                w.Show();
            }
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
            try
            {
                if(dataGrid.SelectedItem!=null)
                {
                
                byte[] bb = stringToByteArray(searchBox.Text);


                MessageBox.Show(byteArrayContaints((dataGrid.SelectedItem as DataPacket).getTeraPacket().data, bb).ToString("X2"));
                }
            }
            catch { MessageBox.Show("ERROR"); }
        }
        private static byte[] stringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        private static int byteArrayContaints(byte[] bb, byte[] what)
        {
            int len = bb.Length - what.Length;
            for (int i = 0; i < len; i++)
            {
                bool flag = true;
                for(int j=0;j<what.Length;j++)
                {
                    if(bb[i+j]!=what[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    return i;
            }
            return -1;
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
                //Медлено по строке
                try
                {
                    byte[] bb = stringToByteArray(searchBox.Text);
                    packs = from p in packs where byteArrayContaints(p.getTeraPacket().data, bb) >=0 select p;
                }
                catch { }
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
            FilterStructure f;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text file (*.xml)|*.xml";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader r = new StreamReader(ofd.OpenFile()))
                {
                    XmlSerializer xsr = new XmlSerializer(typeof(FilterStructure));
                    f = xsr.Deserialize(r) as FilterStructure;
                }

                comboBoxType.SelectedIndex = f.indexRecvSend;
                listBoxWhite.Items.Clear();
                if (f.whiteList != null)
                    foreach (var el in f.whiteList)
                        listBoxWhite.Items.Add(TeraPacketCreator.getOpCode((ushort)el));
                listBoxBlack.Items.Clear();
                if (f.blackList != null)
                    foreach (var el in f.blackList)
                        listBoxBlack.Items.Add(TeraPacketCreator.getOpCode((ushort)el));
            }
        }

        private void buttonFilterExport_Click(object sender, RoutedEventArgs e)
        {
            FilterStructure f = new FilterStructure()
            {
                indexRecvSend = comboBoxType.SelectedIndex,
                whiteList = new object[listBoxWhite.Items.Count],
                blackList = new object[listBoxBlack.Items.Count]
            };
            for (int i = 0; i < f.whiteList.Length; i++)
                f.whiteList[i] = listBoxWhite.Items[i];
            for (int i = 0; i < f.blackList.Length; i++)
                f.blackList[i] = listBoxBlack.Items[i];

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text file (*.xml)|*.xml";
            if(sfd.ShowDialog()==true)
            {
                using (StreamWriter w = new StreamWriter(sfd.OpenFile()))
                {
                    XmlSerializer xsr = new XmlSerializer(f.GetType());
                    xsr.Serialize(w, f);
                }
            }

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

        private void MenuItem_AddWhite_Click(object sender, RoutedEventArgs e)
        {
            AddPacketWindow w = new AddPacketWindow();
            if (w.ShowDialog() == true)
            {
                listBoxWhite.Items.Add(w.valueEnum);
            }
        }

        private void MenuItem_AddBlack_Click(object sender, RoutedEventArgs e)
        {
            AddPacketWindow w = new AddPacketWindow();
            if (w.ShowDialog() == true)
            {
                listBoxBlack.Items.Add(w.valueEnum);
            }
        }

        private void MenuItem_RemoveWhite_Click(object sender, RoutedEventArgs e)
        {
            while(listBoxWhite.SelectedIndex>=0)
            {
                listBoxWhite.Items.RemoveAt(listBoxWhite.SelectedIndex);
            }
        }

        private void MenuItem_RemoveBlack_Click(object sender, RoutedEventArgs e)
        {
            while(listBoxBlack.SelectedIndex >=0 )
            {
                listBoxBlack.Items.RemoveAt(listBoxBlack.SelectedIndex);
            }
        }

        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBlockPacket.Text);
        }
        
        private void checkBoxForTimer_Checked(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
