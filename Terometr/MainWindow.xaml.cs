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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Detrav.Terometr
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

        public void changeTitle(string str)
        {
            this.Title = String.Format("Terometr - {0} - Дпс метр", str);
            this.UpdateLayout();
        }

        public bool close = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !close;
            this.Hide();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowState = System.Windows.WindowState.Normal;
            this.Hide();
        }

        internal void updateDpsList(SortedList<double, Player> list, double dpsMax, ulong selfId)
        {
            while (listBoxDps.Items.Count < list.Count)
                listBoxDps.Items.Add(new PlayerBarElement());
            while (listBoxDps.Items.Count > list.Count)
                listBoxDps.Items.RemoveAt(listBoxDps.Items.Count-1);
            int i = 0;
            foreach(var p in list)
            {
                (listBoxDps.Items[i] as PlayerBarElement).changeData(
                    p.Value.dps / dpsMax,
                    p.Value.name,
                    p.Value.dps.ToString("0:0.00"),
                    p.Value.id == selfId);
                    i++;
            }
        }
    }
}
