using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Terometr.Themes
{
    /// <summary>
    /// Логика взаимодействия для DpsRow.xaml
    /// </summary>
    public partial class DpsRow : UserControl
    {
        public DpsRow()
        {
            InitializeComponent();
        }

        public DpsRow(ulong i, double d, string s1, string s2): this()
        {
            id = i;
            procent = d;
            playerName = s1;
            playerCount = s2;
        }

        public ulong id { get; set; }

        public double procent { get; set; }

        /*public static readonly DependencyProperty procentProperty =
            DependencyProperty.Register("procent",typeof(double),typeof(DpsRow),0.0);*/

        public string playerName { get; set; }

        public string playerCount
        {
            get { return playerCountProperty; }
            set
            {
                playerCountProperty = value;
                this.OnPropertyChanged(playerName);
            }
        }

        public string playerCountProperty { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
