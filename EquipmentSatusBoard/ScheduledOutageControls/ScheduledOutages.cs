using EquipmentSatusBoard.EquipmentControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentSatusBoard.ScheduledOutageControls
{
    public class ScheduledOutages
    {
        private static List<EquipmentScheduledOutage> scheduledOutages;

        internal delegate void NewScheduledOutagesEventHandler(object sender, NewScheduledOutagesEventArgs e);
        internal static event NewScheduledOutagesEventHandler NewScheduledOutages;

        public static void AddOutage(Equipment equipment, ScheduledOutage scheduledOutage)
        {
        }

        internal static void AddOutage(EquipmentScheduledOutage outage)
        {
            if (scheduledOutages == null)
                scheduledOutages = new List<EquipmentScheduledOutage>();

            scheduledOutages.Add(outage);
            scheduledOutages.Sort();

            NewScheduledOutages?.Invoke(null, new NewScheduledOutagesEventArgs() { Outages = scheduledOutages });
        }

        internal static void RemoveOutage(EquipmentScheduledOutage outage)
        {
            if (scheduledOutages == null)
                scheduledOutages = new List<EquipmentScheduledOutage>();

            scheduledOutages.Remove(outage);
            scheduledOutages.Sort();

            NewScheduledOutages?.Invoke(null, new NewScheduledOutagesEventArgs() { Outages = scheduledOutages });

        }
    }

    internal class NewScheduledOutagesEventArgs : EventArgs
    {
        public IEnumerable<EquipmentScheduledOutage> Outages;

        public NewScheduledOutagesEventArgs() { }
    }
}
