using EquipmentSatusBoard.AppModeControls;
using System.Windows;

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for AppModeChangeDialog.xaml
    /// </summary>
    public partial class AppModeChangeDialog : Window
    {
        public AppMode NewMode;

        public AppModeChangeDialog()
        {
            InitializeComponent();
        }

        public AppMode GetModeRequest()
        {
            ShowDialog();
            return NewMode;
        }

        private void SlideClick(object sender, RoutedEventArgs e)
        {
            NewMode = AppMode.Slide;
            Close();
        }

        private void TechClick(object sender, RoutedEventArgs e)
        {
            NewMode = AppMode.Tech;
            Close();
        }

        private void AdminClick(object sender, RoutedEventArgs e)
        {
            NewMode = AppMode.Admin;
            Close();
        }
    }
}
