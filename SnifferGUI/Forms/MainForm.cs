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
                sniffer.start(Config.Instance.adapterNumber);
            }
            else
            {
                Close();
                return;
            }
            //splitContainer1.IsSplitterFixed = true;
            timer1.Enabled = true;
            /*Random rnd = new Random();
            Byte[] b = new Byte[500];
            rnd.NextBytes(b);
            PacketWithStructure.getRtf(ref richTextBox1,b);*/
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
            if(listView1.Items.Count>0)
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            delay = DateTime.Now;
            timer1.Enabled = true;
        }

        private void основныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.numericUpDown1.Value = Config.Instance.packetMaxCount;
            if(settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Config.Instance.packetMaxCount = (int)settingsForm.numericUpDown1.Value;
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


            //currentPacket.data;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ViewValueForm vvForm = new ViewValueForm();
            vvForm.packet = currentPacket;
            vvForm.Show();
        }
    }
}
