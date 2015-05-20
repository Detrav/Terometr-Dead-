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

namespace Detrav.Terometr.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonClose_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Close();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            menuButton_ContextMenuOpening(sender, null);
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void menuButton_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            for(int i = 0; i<tabControl.Items.Count;i++)
            {
                var mm = new MenuItem();
                mm.Header = (tabControl.Items[i] as TabItem).Header;
                mm.Click += (a,b) => {
                    foreach (TabItem t in tabControl.Items)
                        if (t.Header == (a as MenuItem).Header)
                        {
                            tabControl.SelectedItem = t;
                            break;
                        }
                };
                menu.Items.Add(mm);
            }
            (sender as Button).ContextMenu = menu;
        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l = (sender as ListBox);
            if (l.SelectedIndex >= 0)
                MessageBox.Show("test");
            l.SelectedIndex = -1;
        }

    }
}
