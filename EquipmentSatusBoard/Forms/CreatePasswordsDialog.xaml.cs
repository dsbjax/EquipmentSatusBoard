using System.Windows;

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for CreatePasswordsDialog.xaml
    /// </summary>
    public partial class CreatePasswordsDialog : Window
    {
        private const string BLANK = "";

        public string AdminPassword { get { return adminPassword1.Password; } }
        public string TechPassword { get { return techPassword1.Password; } }

        public CreatePasswordsDialog()
        {
            InitializeComponent();
            adminPassword1.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            ok.IsEnabled =
                !adminPassword1.Password.Equals(BLANK) &
                !techPassword1.Password.Equals(BLANK) &
                adminPassword1.Password.Equals(adminPassword2.Password) &
                techPassword1.Password.Equals(techPassword2.Password);
        }
    }
}