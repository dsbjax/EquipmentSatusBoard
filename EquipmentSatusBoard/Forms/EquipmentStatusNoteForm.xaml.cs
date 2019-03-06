using System;
using System.Windows;

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for EquipmentStatusNoteForm.xaml
    /// </summary>
    public partial class EquipmentStatusNoteForm : Window
    {
        public String Note { get { return note.Text; } }

        public EquipmentStatusNoteForm()
        {
            InitializeComponent();
            note.Focus();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
