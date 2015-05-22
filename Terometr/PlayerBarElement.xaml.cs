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
    /// Логика взаимодействия для PlayerBarElement.xaml
    /// </summary>
    public partial class PlayerBarElement : UserControl
    {
        public PlayerBarElement()
        {
            InitializeComponent();
            green = (Brush)br.ConvertFrom("#FF10AE00");
            blue = (Brush)br.ConvertFrom("#FF1000AE");
        }

        BrushConverter br = new BrushConverter();
        Brush green;
        Brush blue;
        

        public void changeData(double progressValue,string left,string right, bool me = false)
        {
            if (me) progressBar.Foreground = green;
            else progressBar.Foreground = blue;
            try { progressBar.Value = progressValue; }
            catch { };
            labelLeft.Content = left;
            labelRight.Content = right;
        }
    }
}
