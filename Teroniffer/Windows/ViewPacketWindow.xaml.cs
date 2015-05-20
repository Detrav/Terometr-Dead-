using Detrav.Sniffer.Tera;
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
using System.Windows.Shapes;

namespace Detrav.Teroniffer.Windows
{
    /// <summary>
    /// Логика взаимодействия для ViewPacketWindow.xaml
    /// </summary>
    public partial class ViewPacketWindow : Window
    {
        public ViewPacketWindow()
        {
            InitializeComponent();
            comboBoxType.ItemsSource = new string[]
            {
                "bitarray",
                "byte",
                "sbyte",
                "ushort",
                "short",
                "uint",
                "int",
                "ulong",
                "long",
                "float",
                "double",
                "char",
                "string",
                "boolean",
                "hex"
            };
            comboBoxType.SelectedIndex = 0;
        }

        byte[] data;

        public void setData(byte[] data)
        {
            this.data = data;
            reMake();
        }
        public void reMake()
        {
            try
            {
                string str = "{0:X4} - {1} : {2}\n\n{3}";
                string str_with_shift = "{0:X4}+{1} - {2} : {3}\n\n{4}";
                try
                {
                    ushort start = UInt16.Parse(numericBoxStart.Text);
                    ushort size = UInt16.Parse(numericBoxSize.Text);
                    string type = comboBoxType.SelectedItem.ToString();
                    switch (type)
                    {
                        case "bitarray":
                            BitArray ba = new BitArray(new byte[1] { data[start] });
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < ba.Length; i++)
                                if (ba[i]) sb.Append("1");
                                else
                                    sb.Append("0");
                            str = String.Format(str, start, sb.ToString(), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 1));
                            break;
                        case "byte": str = String.Format(str, start, data[start], type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 1)); break;
                        case "sbyte": str = String.Format(str, start, (sbyte)data[start], type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 1)); break;
                        case "ushort": str = String.Format(str, start, BitConverter.ToUInt16(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 2)); break;
                        case "short": str = String.Format(str, start, BitConverter.ToInt16(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 2)); break;
                        case "uint": str = String.Format(str, start, BitConverter.ToUInt32(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 4)); break;
                        case "int": str = String.Format(str, start, BitConverter.ToInt32(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 4)); break;
                        case "ulong": str = String.Format(str, start, BitConverter.ToUInt64(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 8)); break;
                        case "long": str = String.Format(str, start, BitConverter.ToInt64(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 8)); break;
                        case "float": str = String.Format(str, start, BitConverter.ToSingle(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 4)); break;
                        case "double": str = String.Format(str, start, BitConverter.ToDouble(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 8)); break;
                        case "char": str = String.Format(str, start, BitConverter.ToChar(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 1)); break;
                        case "string": str = String.Format(str, start, TeraPacketParser.byteArrayToString(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, TeraPacketParser.byteArrayToString(data, start).Length)); break;
                        case "boolean": str = String.Format(str, start, BitConverter.ToBoolean(data, start), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, 1)); break;
                        case "hex": str = String.Format(str, start, TeraPacketParser.byteArrayToHexString(data, start, size), type, TeraPacketParser.byteArrayToHexStringRightToLeft(data, start, size)); break;
                        default: str = String.Format(str_with_shift, start, size, "unknown", type); break;
                    }
                }
                catch (Exception e)
                {
                    str = e.Message;
                }
                textBox.Text = str;
            }
            catch { }
        }

        private void Button_Click_plus1(object sender, RoutedEventArgs e)
        {
            int v = 0;
            Int32.TryParse(numericBoxStart.Text, out v);
            numericBoxStart.Text = (v+1).ToString();
        }
        private void Button_Click_minus1(object sender, RoutedEventArgs e) {
            int v = 0;
            Int32.TryParse(numericBoxStart.Text, out v);
            numericBoxStart.Text = (v-1).ToString();
        }
        private void Button_Click_plus2(object sender, RoutedEventArgs e) {
            int v = 0;
            Int32.TryParse(numericBoxSize.Text, out v);
            numericBoxSize.Text = (v+1).ToString();
        }
        private void Button_Click_minus2(object sender, RoutedEventArgs e) {
            int v = 0;
            Int32.TryParse(numericBoxSize.Text, out v);
            numericBoxSize.Text = (v-1).ToString();
        }

        private void numericBoxStart_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Up)
            {
                int v = 0;
                Int32.TryParse(numericBoxStart.Text, out v);
                numericBoxStart.Text = (v + 1).ToString();
            }
            else if( e.Key == Key.Down)
            {
                int v = 0;
                Int32.TryParse(numericBoxStart.Text, out v);
                numericBoxStart.Text = (v - 1).ToString();
            }
        }

        private void numericBoxSize_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                int v = 0;
                Int32.TryParse(numericBoxSize.Text, out v);
                numericBoxSize.Text = (v + 1).ToString();
            }
            else if (e.Key == Key.Down)
            {
                int v = 0;
                Int32.TryParse(numericBoxSize.Text, out v);
                numericBoxSize.Text = (v - 1).ToString();
            }
        }

        private void numericBoxSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            reMake();
        }

        private void numericBoxStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            reMake();
        }

        private void comboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reMake();
        }
    }
}
