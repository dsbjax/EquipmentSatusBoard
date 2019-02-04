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
