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
            init.comboBox1.SelectedIndex = 0;
            if(init.ShowDialog()== System.Windows.Forms.DialogResult.Cancel)
            {
                Close();
                return;
            }
            else
            {
                sniffer.serverIp = init.comboBox2.Text;
                sniffer.start(init.comboBox1.SelectedIndex);
            }
        }

        void sniffer_onParsePacket(Connection connection, TeraPacket packet)
        {
            this.Invoke(new Action<ushort>((size) => {label1.Text = (long.Parse(label1.Text)+size).ToString();}),packet.size);
        }

        

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sniffer.stop();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
