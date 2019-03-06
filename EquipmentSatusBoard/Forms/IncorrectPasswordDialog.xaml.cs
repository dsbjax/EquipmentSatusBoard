using System.Windows;

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for IncorrectPasswordDialog.xaml
    /// </summary>
    public partial class IncorrectPasswordDialog : Window
    {
        public IncorrectPasswordDialog()
        {
            InitializeComponent();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
