using EquipmentSatusBoard.AppModeControls;
using System.Windows;

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
