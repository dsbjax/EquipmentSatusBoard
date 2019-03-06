using System.Windows;

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
