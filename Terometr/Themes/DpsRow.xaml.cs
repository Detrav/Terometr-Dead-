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

        public DpsRow(double d, string s1, string s2): this()
        {
            procent = d;
            playerName = s1;
            playerCount = s2;
        }

        public double procent { get; set; }

        /*public static readonly DependencyProperty procentProperty =
            DependencyProperty.Register("procent",typeof(double),typeof(DpsRow),0.0);*/

        public string playerName { get; set; }

        public string playerCount { get; set; }
    }
}
