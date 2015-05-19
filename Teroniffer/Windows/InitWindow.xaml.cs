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
    /// Логика взаимодействия для InitWindow.xaml
    /// </summary>
    public partial class InitWindow : Window
    {
        public int selectedIndex { get; private set; }
        public InitWindow(string[] list)
        {
            InitializeComponent();
            foreach (var el in list)
                listBox.Items.Add(el);
        }

        private void buttonCansel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex < 0)
            {
                System.Windows.MessageBox.Show("Нужно выбрать одно из устройств!");
                return;
            }
            selectedIndex = listBox.SelectedIndex;
            DialogResult = true;
        }
    }
}
