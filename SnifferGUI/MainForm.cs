using Sniffer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnifferGUI
{
    public partial class MainForm : Form
    {
        Capture sniffer;
        int dataCountInt = 0;
        int inPacketCountInt = 0;
        int outPacketCountInt = 0;
        DateTime delay = DateTime.Now;
        List<TeraPacket> packets = new List<TeraPacket>();
        string[] packetName = Config.Instance.packetName;

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
        }

        void sniffer_onParsePacket(Connection connection, TeraPacket packet)
        {
            dataCountInt += packet.size;
            if (packet.type == TeraPacket.Type.Recv) inPacketCountInt++;
            else outPacketCountInt++;
            lock(packets)
            {
                packets.Add(packet);
            }
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
                        if(p.type == TeraPacket.Type.Recv)
                            listView1.Items.Add(new ListViewItem(new string[]{"in",p.size.ToString(),packetName[p.opCode]}));
                        else
                            listView1.Items.Add(new ListViewItem(new string[]{"out",p.size.ToString(),packetName[p.opCode]}));
                    }
                }
                while(packets.Count>Config.Instance.packetMaxCount)
                {
                    packets.RemoveAt(0);
                    listView1.Items.RemoveAt(0);
                }
            }
            
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
    }
}
