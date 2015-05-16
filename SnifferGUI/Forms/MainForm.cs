using Sniffer;
using Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SnifferGUI.Forms
{
    public partial class MainForm : Form
    {
        Capture sniffer;
        int dataCountInt = 0;
        int inPacketCountInt = 0;
        int outPacketCountInt = 0;
        DateTime delay = DateTime.Now;
        List<TeraPacket> packets = new List<TeraPacket>();
        ushort[] whiteList = (ushort[])Config.Instance.whiteList.Clone();//Не забывать их обновлять и делать lock
        bool whiteListEnable = Config.Instance.whiteListEnable;//
        bool blackListEnable = Config.Instance.blackListEnable;//
        ushort[] blackList = (ushort[])Config.Instance.blackList.Clone();//Не забывать их обновлять и делать lock
        bool captureEnable = true; //Идёт ди запись
        TeraPacketParser currentPacket;//Текущий пакет, для просмотра
        //Поиск Hex значений по пакетам
        byte[] hexSearch;
        int hexSearchNum = 0;

        public MainForm()
        {
            InitializeComponent();
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sniffer = new Capture();
            sniffer.onParsePacket += sniffer_onParsePacket;
            InitForm init = new InitForm();
            string[] devices = sniffer.getDevices();
            if (devices != null)
                init.comboBox1.Items.AddRange(devices);
            init.comboBox1.SelectedIndex = Config.Instance.adapterNumber;
            init.comboBox2.Text = Config.Instance.serverIp;
            init.comboBox1.SelectedIndex = 0;
            
            if (init.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Config.Instance.serverIp = init.comboBox2.Text;
                Config.Instance.adapterNumber = init.comboBox1.SelectedIndex;
                Config.saveConfig();
                sniffer.serverIp = Config.Instance.serverIp;
                sniffer.flagToDebug = Config.Instance.flagToDebug;
                sniffer.flagToPacketLog = Config.Instance.flagToPacketLog;
                sniffer.flagToSnifferLog = Config.Instance.flagToSnifferLog;
                sniffer.start(Config.Instance.adapterNumber);
            }
            else
            {
                Close();
                return;
            }
            //splitContainer1.IsSplitterFixed = true;
            timer1.Enabled = true;
            
            Random rnd = new Random();
            Byte[] b = new Byte[500];
            rnd.NextBytes(b);
            packets.Add(TeraPacketCreator.create(new TeraPacket(b, TeraPacket.Type.Recv)));
        }

        void sniffer_onParsePacket(Connection connection, TeraPacket packet)
        {
            if (!captureEnable) return;
            lock(packets)
            {
                if(whiteListEnable)
                {
                    if (whiteList == null) return;
                    if (!whiteList.Contains(packet.opCode)) return;
                }
                if(blackListEnable)
                {
                    if (blackList == null) return;
                    if (blackList.Contains(packet.opCode)) return;
                }
                packets.Add(packet);
            }
            dataCountInt += packet.size;
            if (packet.type == TeraPacket.Type.Recv) inPacketCountInt++;
            else outPacketCountInt++;
            /*
             * Как вариант тут можно сделать инвоке с жутким делегатом или сделать внешний обработчик и по таймеру обновлять форму
             * this.Invoke(new Action<ushort>((size) => {label1.Text = (long.Parse(label1.Text)+size).ToString();}),packet.size);
             */
        }

        

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sniffer.stop();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            float dataCountFloat = (float)dataCountInt;
            int sizeNum = 0;
            while(dataCountFloat>1024)
            {
                dataCountFloat /=1024f;
                sizeNum++;
            }
            switch (sizeNum)
            {
                case 0:
                    dataCount.Text = String.Format("Объём данных: {0} байт", dataCountInt);
                    break;
                case 1:
                    dataCount.Text = String.Format("Объём данных: {0:##.##} Кб", dataCountFloat);
                    break;
                case 2:
                    dataCount.Text = String.Format("Объём данных: {0:##.##} Мб", dataCountFloat);
                    break;
                case 3:
                    dataCount.Text = "Объём данных: >1 Гб";
                    break;
            }
            inPacketCount.Text = String.Format("Входящих пакетов: {0,7}", inPacketCountInt);
            outPacketCount.Text = String.Format("Исходящих пакетов: {0,5}", outPacketCountInt);
            responseLabel.Text = String.Format("Отклик: {0,5} мс", (DateTime.Now - delay).Milliseconds);
            
            lock(packets)
            {
                if(listView1.Items.Count< packets.Count)
                {
                    foreach(var p in packets.Skip(listView1.Items.Count))
                    {
                        if (p.type == TeraPacket.Type.Recv)
                            listView1.Items.Add(new ListViewItem(new string[] { "in", p.size.ToString(), Config.getPacketName(p.opCode) }));
                        else
                            listView1.Items.Add(new ListViewItem(new string[] { "out", p.size.ToString(), Config.getPacketName(p.opCode) }));
                    }
                }
                while(packets.Count>Config.Instance.packetMaxCount)
                {
                    packets.RemoveAt(0);
                    listView1.Items.RemoveAt(0);
                }
            }
            if (listView1.Items.Count > 0)
                if (captureEnable)
                    listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            delay = DateTime.Now;
            timer1.Enabled = true;
        }

        private void основныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.numericUpDown1.Value = Config.Instance.packetMaxCount;
            settingsForm.checkBoxForDebug.Checked = Config.Instance.flagToDebug;
            settingsForm.checkBoxForPacketLog.Checked = Config.Instance.flagToPacketLog;
            settingsForm.checkBoxForSnifferLog.Checked = Config.Instance.flagToSnifferLog;
            if(settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Config.Instance.packetMaxCount = (int)settingsForm.numericUpDown1.Value;
                Config.Instance.flagToDebug = settingsForm.checkBoxForDebug.Checked;
                Config.Instance.flagToPacketLog = settingsForm.checkBoxForPacketLog.Checked;
                Config.Instance.flagToSnifferLog = settingsForm.checkBoxForSnifferLog.Checked;
                Config.saveConfig();
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void фильтрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltersForm filtersForm = new FiltersForm();
            filtersForm.checkBoxBlackList.Checked = Config.Instance.blackListEnable;
            filtersForm.checkBoxWhiteList.Checked = Config.Instance.whiteListEnable;
            //Первый раз так пишу, надеюсь не будет краша изза нуля при второй проверке
            if (Config.Instance.blackList != null && Config.Instance.blackList.Length > 0)
            {
                foreach (var el in Config.Instance.blackList)
                    filtersForm.listBoxBlack.Items.Add(Config.getPacketName(el));
                filtersForm.sortListBox(ref filtersForm.listBoxBlack);

                //foreach (var el in Config.getPacketsName())
                for (ushort i = 0; i < ushort.MaxValue;i++ )
                    if (Config.isPacket(i))
                        if (!Config.Instance.blackList.Contains(i))
                            filtersForm.listBoxPacketsNameForBlack.Items.Add(Config.getPacketName(i));
                filtersForm.sortListBox(ref filtersForm.listBoxPacketsNameForBlack);
            }
            else
            {
                for (ushort i = 0; i < ushort.MaxValue; i++)
                    if (Config.isPacket(i))
                        filtersForm.listBoxPacketsNameForBlack.Items.Add(Config.getPacketName(i));
                filtersForm.sortListBox(ref filtersForm.listBoxPacketsNameForBlack);
            }
            if(Config.Instance.whiteList !=null && Config.Instance.whiteList.Length > 0)
            {
                foreach (var el in Config.Instance.whiteList)
                    filtersForm.listBoxWhite.Items.Add(Config.getPacketName(el));
                filtersForm.sortListBox(ref filtersForm.listBoxWhite);

                //foreach (var el in Config.getPacketsName())
                for (ushort i = 0; i < ushort.MaxValue; i++)
                    if (Config.isPacket(i))
                        if (!Config.Instance.whiteList.Contains(i))
                            filtersForm.listBoxPacketsNameForWhite.Items.Add(Config.getPacketName(i));
                filtersForm.sortListBox(ref filtersForm.listBoxPacketsNameForWhite);
            }
            else
            {
                for (ushort i = 0; i < ushort.MaxValue; i++)
                    if (Config.isPacket(i))
                        filtersForm.listBoxPacketsNameForWhite.Items.Add(Config.getPacketName(i));
                filtersForm.sortListBox(ref filtersForm.listBoxPacketsNameForWhite);
            }
            if(filtersForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Config.Instance.blackListEnable = filtersForm.checkBoxBlackList.Checked;
                Config.Instance.whiteListEnable = filtersForm.checkBoxWhiteList.Checked;
                if (filtersForm.listBoxBlack.Items.Count > 0)
                {
                    List<string> filter = new List<string>();
                    foreach (string el in filtersForm.listBoxBlack.Items)
                        filter.Add(el);
                    Config.Instance.blackList = Config.getArrayOfPacketsName(filter.ToArray());
                }
                if (filtersForm.listBoxWhite.Items.Count > 0)
                {
                    List<string> filter = new List<string>();
                    foreach (string el in filtersForm.listBoxWhite.Items)
                        filter.Add(el);
                    Config.Instance.whiteList = Config.getArrayOfPacketsName(filter.ToArray());
                }
                Config.saveConfig();
                if (Config.Instance.whiteList!=null)
                lock (whiteList)
                {
                    whiteList = (ushort[])Config.Instance.whiteList.Clone();
                }
                whiteListEnable = Config.Instance.whiteListEnable;
                if (Config.Instance.blackList != null)
                lock (blackList)
                {
                    blackList = (ushort[])Config.Instance.blackList.Clone();
                }
                blackListEnable = Config.Instance.blackListEnable;
            }
        }

        private void toolStripButtonCaptureEnable_Click(object sender, EventArgs e)
        {
                        captureEnable = !captureEnable;
            if(captureEnable)
                toolStripButtonCaptureEnable.Image = Properties.Resources.check;
            else
                toolStripButtonCaptureEnable.Image = Properties.Resources.cross;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems == null) { panelPacketView.Enabled = false; currentPacket = null; return; }
            if (listView1.SelectedItems.Count == 0) { panelPacketView.Enabled = false; currentPacket = null; return; }
            lock(packets)
            {
                currentPacket = TeraPacketCreator.create(packets[listView1.SelectedItems[0].Index]);
            }
            panelPacketView.Enabled = true;
            richTextBox1.Text = currentPacket.ToString();
            reColorRtfBySearch();

            //currentPacket.data;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ViewValueForm vvForm = new ViewValueForm();
            vvForm.packet = currentPacket;
            vvForm.Show();
        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            if (captureEnable) return;
            try { hexSearch = stringToByteArray(toolStripTextBoxForSearch.Text); }
            catch { hexSearch = null; }
            if (hexSearch == null) return;

            for (; hexSearchNum < packets.Count; hexSearchNum++)
                if (searchByteArrayinByteArray(packets[hexSearchNum].data, hexSearch))
                {
                    listView1.SelectedIndices.Clear();
                    listView1.SelectedIndices.Add(hexSearchNum);
                    hexSearchNum++;
                    if (hexSearchNum >= packets.Count-1) hexSearchNum = 0;
                    return;
                }
            if (hexSearchNum >= packets.Count) hexSearchNum = 0;
            MessageBox.Show("Ничего не найдено");
        }

        private void reColorRtfBySearch()
        {
            if (hexSearch == null) return;
            for (int i = 0; i < currentPacket.data.Length; i++)
            {
                if (currentPacket.data.Length - i < hexSearch.Length)
                    return;
                bool flag = true;
                for (int j = 0; j < hexSearch.Length; j++)
                {
                    if (currentPacket.data[i + j] != hexSearch[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    for (int j = 0; j < hexSearch.Length; j++)
                        highlight(i + j, Color.Black, Color.White);
                }
            }
        }

        private void highlight(int p, Color color1, Color color2)
        {
            /*
Offset 00 01 02 03 04 05 06 07 | 08 09 0A 0B 0C 0D 0E 0F  0123456789ABCDEF
 0000: 87 05 F9 6E E1 D6 2F E4 | 2D C6 E7 0C DC F7 D6 73  ..ùnáÖ/ä-Æç.Ü÷Ös
 0010: 52 87 65 3B 4D 82 04 AB | 12 9C BA 4C 98 43 7E 1E  R.e;M..«..ºL.C~.
 0020: 3F 89 39 A3 DF F0 B4 7F | B7 C6 8C BB D8 30 31 EF  ?.9£ßð´.·Æ.»Ø01ï
 0030: D3 65 F4 3C A4 DE 83 04 | 00 4D 5B 45 1C E3 4B EA  Óeô<¤Þ...M[E.ãKê
 0040: 48 C9 CD 52 BE 33 93 61 | D3 E5 83 87 F6 48 D3 E1  HÉÍR¾3.aÓå..öHÓá
 0050: 6A CA 78 33 4E 41 79 60 | 06 34 A2 91 95 6E 63 D4  jÊx3NAy`.4¢..ncÔ
 0060: 7A A1 72 0F 63 DE 31 A0 | D2 92 68 BF 5A FB 0E B8  z¡r.cÞ1 Ò.h¿Zû.¸
             */
            int row = p / 16;
            int start = (1 + row) * 75 + 7;
            int col = p % 16;
            if (col >= 8)
            { start += 26; col -= 8; }
            start += col * 3;
            richTextBox1.SelectionStart = start;
            richTextBox1.SelectionLength = 2;
            richTextBox1.SelectionColor = color2;
            richTextBox1.SelectionBackColor = color1;
        }

        private void toolStripTextBoxForSearch_TextChanged(object sender, EventArgs e)
        {
            
        }

        public static byte[] stringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static bool searchByteArrayinByteArray(byte[] buffer,byte[] search, int start = 0)
        {
            for(int i = start;i<buffer.Length;i++)
            {
                if (buffer.Length - i < search.Length)
                    return false;
                bool flag = true;
                for(int j = 0; j<search.Length;j++)
                {
                    if (buffer[i + j] != search[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag) return true;
            }
            return false;
        }
    }
}
