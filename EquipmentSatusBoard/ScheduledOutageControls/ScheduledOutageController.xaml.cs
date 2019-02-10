using EquipmentSatusBoard.EquipmentControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EquipmentSatusBoard.ScheduledOutageControls
{
    /// <summary>
    /// Interaction logic for ScheduledOutageController.xaml
    /// </summary>
    public partial class ScheduledOutageController : UserControl
    {
        ObservableCollection<EquipmentScheduledOutage> itemsList = new ObservableCollection<EquipmentScheduledOutage>();

        public ScheduledOutageController()
        {
            InitializeComponent();

            outages.ItemsSource = itemsList;
            ScheduledOutages.NewScheduledOutages += NewScheduledOutages;
        }

        private void NewScheduledOutages(object sender, NewScheduledOutagesEventArgs e)
        {
            itemsList.Clear();
            foreach (EquipmentScheduledOutage outage in e.Outages)
                itemsList.Add(outage);
        }
    }
}
