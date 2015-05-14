using Sniffer.Tera;
using System;
using System.Collections;
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
    public partial class ViewValueForm : Form
    {
        public ViewValueForm()
        {
            InitializeComponent();
        }
        public TeraPacket packet;

        void reSetRtf()
        {
            string str = "{0:X4} - {1} : {2}\n\n{3}";
            string str_with_shift = "{0:X4}+{1} - {2} : {3}\n\n{4}";
            try
            {
                ushort start = (ushort)numericUpDownByteNumber.Value;
                ushort size = (ushort)numericUpDownSize.Value;
                string type = comboBoxDataType.Text;
                switch (type)
                {
                    case "bitarray":
                        BitArray ba = new BitArray(new byte[1] { packet.data[start] });
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < ba.Length; i++)
                            if (ba[i]) sb.Append("1");
                            else
                                sb.Append("0");
                        str = String.Format(str, start, sb.ToString(), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 1));
                        break;
                    case "byte": str = String.Format(str, start, packet.data[start], type,TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data,start,1)); break;
                    case "sbyte": str = String.Format(str, start, (sbyte)packet.data[start], type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 1)); break;
                    case "ushort": str = String.Format(str, start, BitConverter.ToUInt16(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 2)); break;
                    case "short": str = String.Format(str, start, BitConverter.ToInt16(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 2)); break;
                    case "uint": str = String.Format(str, start, BitConverter.ToUInt32(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 4)); break;
                    case "int": str = String.Format(str, start, BitConverter.ToInt32(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 4)); break;
                    case "ulong": str = String.Format(str, start, BitConverter.ToUInt64(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 8)); break;
                    case "long": str = String.Format(str, start, BitConverter.ToInt64(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 8)); break;
                    case "float": str = String.Format(str, start, BitConverter.ToSingle(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 4)); break;
                    case "double": str = String.Format(str, start, BitConverter.ToDouble(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 8)); break;
                    case "char": str = String.Format(str, start, BitConverter.ToChar(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 1)); break;
                    case "string": str = String.Format(str, start, TeraPacketParser.byteArrayToString(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, TeraPacketParser.byteArrayToString(packet.data, start).Length)); break;
                    case "boolean": str = String.Format(str, start, BitConverter.ToBoolean(packet.data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, 1)); break;
                    case "hex": str = String.Format(str, start, TeraPacketParser.byteArrayToHexString(packet.data, start, size), type, TeraPacketParser.byteArrayToHexStringRightToLeft(packet.data, start, size)); break;
                    default: str = String.Format(str_with_shift, start, size, "unknown", type); break;
                }
            }
            catch(Exception e)
            {
                str = e.Message;
            }
            richTextBox1.Text = str;
        }

        private void numericUpDownByteNumber_ValueChanged(object sender, EventArgs e)
        {
            reSetRtf();
        }

        private void numericUpDownSize_ValueChanged(object sender, EventArgs e)
        {
            reSetRtf();
        }

        private void comboBoxDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            reSetRtf();
        }

        private void ViewValueForm_Shown(object sender, EventArgs e)
        {
            reSetRtf();
        }

    }
}
