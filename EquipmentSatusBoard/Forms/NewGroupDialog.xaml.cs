using System.Windows;

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for NewGroupDialog.xaml
    /// </summary>
    public partial class NewGroupDialog : Window
    {
        internal string GroupName
        {
            get { return groupName.Text; }
        }

        internal int EquipmentCount
        {
            get { return (int)equipmentCount.Value; }
        }

        public NewGroupDialog()
        {
            InitializeComponent();

            groupName.Focus();
            groupName.SelectAll();
        }

        private void CreateGroupClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
