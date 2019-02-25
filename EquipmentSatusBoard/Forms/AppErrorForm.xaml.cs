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

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for AppErrorForm.xaml
    /// </summary>
    public partial class AppErrorForm : Window
    {
        public AppErrorForm()
        {
            InitializeComponent();
        }

        public AppErrorForm(string errorMessage)
        {
            this.errorMessage.Content = errorMessage;
        }
    }
}
