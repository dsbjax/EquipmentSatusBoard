using System.Collections.ObjectModel;
using System.Windows.Controls;

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
