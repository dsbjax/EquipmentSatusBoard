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
