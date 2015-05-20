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

namespace Detrav.Teroniffer.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddPacketWindow.xaml
    /// </summary>
    public partial class AddPacketWindow : Window
    {
        public AddPacketWindow()
        {
            InitializeComponent();
            SortedList<string, object> sortedList = new SortedList<string, object>();
            foreach (var i in Detrav.Sniffer.Tera.TeraPacketCreator.getEnumList())
                sortedList.Add(i.ToString(), i);
            listBox.ItemsSource = null;
            listBox.ItemsSource = sortedList.Values;            
        }

        public object valueEnum { get; set; }

        private void buttonCansel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
            {
                DialogResult = false;
                return;
            }
            valueEnum = listBox.SelectedItem;
            DialogResult = true;
        }
    }
}
