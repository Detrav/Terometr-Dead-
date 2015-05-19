using System;
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

namespace Teroniffer.Windows
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

        }
    }
}
