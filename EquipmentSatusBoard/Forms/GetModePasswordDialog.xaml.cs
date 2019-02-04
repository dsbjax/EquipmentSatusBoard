using EquipmentSatusBoard.AppModeControls;
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
    /// Interaction logic for GetModePasswordDialog.xaml
    /// </summary>
    public partial class GetModePasswordDialog : Window
    {
        public GetModePasswordDialog()
        {
            InitializeComponent();
        }

        public string ShowDialog(AppMode mode)
        {
            title.Content += mode.ToString();

            password.Focus();
            ShowDialog();

            return password.Password;
        }

        private void EnterClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
