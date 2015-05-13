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
            delay = DateTime.Now;
            timer1.Enabled = true;
        }
    }
}
