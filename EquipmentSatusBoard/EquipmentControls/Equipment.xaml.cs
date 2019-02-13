using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.Forms;
using EquipmentSatusBoard.ScheduledOutageControls;
using EquipmentSatusBoard.StatusBoardControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EquipmentSatusBoard.EquipmentControls
{
    /// <summary>
    /// Interaction logic for Equipment.xaml
    /// </summary>
    public partial class Equipment : UserControl, IAppMode
    {
        private List<EquipmentScheduledOutage> scheduledOutages = new List<EquipmentScheduledOutage>();
        private EquipmentStatus status;
        private bool initComplete = false;

        public object EquipmentName { get { return slideEquipmentLabel.Content; } }

        internal delegate void EquipmentDeleteEventHandle(object sender, EquipmentDeleteEventArgs e);
        internal event EquipmentDeleteEventHandle EquipmentDelete;

        public Equipment()
        {
            InitializeComponent();

            AppModeNotifications.Subscribe(this);
            SetMode(AppMode.Slide);

            adminEquipmentTextBox.GotFocus += AdminEquipmentTextBoxGotFocus;

            SetMenuItemTags();

            initComplete = true;
        }

        public Equipment(string equipmentName, StreamReader statusLines)
        {
            InitializeComponent();

            adminEquipmentTextBox.GotFocus += AdminEquipmentTextBoxGotFocus;

            SetMenuItemTags();

            slideEquipmentLabel.Content = techEquipmentLabel.Content = adminEquipmentTextBox.Text = equipmentName;
            var equipmentOptions = statusLines.ReadLine().Split(',');

            string temp = equipmentOptions[0].Substring(equipmentOptions[0].LastIndexOf(' ') + 1);
            switch (temp)
            {
                case "Operational":
                    EquipmentStatusClick(operationalMenuItem, new RoutedEventArgs());
                    break;

                case "Degraded":
                    EquipmentStatusClick(degradedMenuItem, new RoutedEventArgs());
                    break;

                case "Down":
                    EquipmentStatusClick(unscheduledMenuItem, new RoutedEventArgs());
                    break;

                case "Scheduled":
                    EquipmentStatusClick(scheduleMenuItem, new RoutedEventArgs());
                    break;
            }

            if (equipmentOptions[1].Contains("On"))
                OperationalStatusClick(onLineMenuItem, new RoutedEventArgs());
            else
                OperationalStatusClick(offLineMenuItem, new RoutedEventArgs());

            noteText.Text = statusLines.ReadLine().Replace("Equip Status Note = ", "");
            if (noteText.Text != "")
                EquipmentStatusScrollingMarquee.AddEquipmentStatusText(EquipmentName + ": " + noteText.Text);

            string line;
            while(!(line = statusLines.ReadLine()).Equals("End Equipment"))
            {
                var outageOptions = line.Split(',');
                var importedOutage = new ScheduledOutage()
                {
                    OutageStart = new DateTime(
                        int.Parse(outageOptions[0].Substring(outageOptions[0].Length - 4)),
                        int.Parse(outageOptions[0].Substring(outageOptions[0].Length - 8, 2)),
                        int.Parse(outageOptions[0].Substring(outageOptions[0].Length - 6, 2)),
                        int.Parse(outageOptions[1].Substring(outageOptions[1].Length - 4, 2)),
                        int.Parse(outageOptions[1].Substring(outageOptions[1].Length - 2)), 0, DateTimeKind.Utc),

                    OutageEnd = new DateTime(
                        int.Parse(outageOptions[2].Substring(outageOptions[2].Length - 4)),
                        int.Parse(outageOptions[2].Substring(outageOptions[2].Length - 8, 2)),
                        int.Parse(outageOptions[2].Substring(outageOptions[2].Length - 6, 2)),
                        int.Parse(outageOptions[3].Substring(outageOptions[3].Length - 4, 2)),
                        int.Parse(outageOptions[3].Substring(outageOptions[3].Length - 2)), 0, DateTimeKind.Utc),

                    Notes = outageOptions[4].Substring(9)
                };

                var outage = new EquipmentScheduledOutage(this, importedOutage);

                scheduledOutages.Add(outage);
                ScheduledOutages.AddOutage(outage);
            }

            AppModeNotifications.Subscribe(this);
            SetMode(AppMode.Slide);

            adminEquipmentTextBox.GotFocus += AdminEquipmentTextBoxGotFocus;

            SetMenuItemTags();

            initComplete = true;
        }

        private void AddEditEquipmentStatusNote()
        {
            var form = new EquipmentStatusNoteForm();
            form.ShowDialog();

            if (form.Note != "")
            {
                if (noteText.Text != "")
                    EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(EquipmentName + ": " + noteText.Text);

                EquipmentStatusScrollingMarquee.AddEquipmentStatusText(EquipmentName + ": " + form.Note);
                noteText.Text = form.Note;
            }
        }

        internal void EndScheduledOutage()
        {
            EquipmentStatusClick(operationalMenuItem, new RoutedEventArgs());
            OperationalStatusClick(onLineMenuItem, new RoutedEventArgs());
        }

        internal void RemoveOutage(EquipmentScheduledOutage outage)
        {
            scheduledOutages.Remove(outage);
        }

        internal void StartScheduledOutage()
        {
            EquipmentStatusClick(scheduleMenuItem, new RoutedEventArgs());
        }

        private void AdminEquipmentTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            adminEquipmentTextBox.SelectAll();
        }

        private void SetMenuItemTags()
        {
            operationalMenuItem.Tag = EquipmentStatus.Operational;
            degradedMenuItem.Tag = EquipmentStatus.Degraded;
            downMenuItem.Tag = EquipmentStatus.Down;
            scheduleMenuItem.Tag = EquipmentStatus.Scheduled;
            unscheduledMenuItem.Tag = EquipmentStatus.Down;

            onLineMenuItem.Tag = OperationalStatus.OnLine;
            offLineMenuItem.Tag = OperationalStatus.OffLine;
        }

        public void SetMode(AppMode newMode)
        {
            slideEquipmentLabel.Visibility = newMode == AppMode.Slide ? Visibility.Visible : Visibility.Collapsed;
            techEquipmentLabel.Visibility = newMode == AppMode.Tech ? Visibility.Visible : Visibility.Collapsed;
            adminDeleteButton.Visibility = adminEquipmentTextBox.Visibility = newMode == AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AdminEquipmentTextChanged(object sender, TextChangedEventArgs e)
        {
            slideEquipmentLabel.Content = techEquipmentLabel.Content = adminEquipmentTextBox.Text;
        }

        private void EquipmentStatusClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item != null)
                switch (status = (EquipmentStatus)item.Tag)
                {
                    case EquipmentStatus.Operational:
                        equipmentStatusBackground.Fill = Brushes.Green;
                        if (initComplete && noteText.Text != "")
                            EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(EquipmentName + ": " + noteText.Text);

                        noteText.Text = "";
                        break;

                    case EquipmentStatus.Degraded:
                        equipmentStatusBackground.Fill = Brushes.Orange;
                        if (initComplete) AddEditEquipmentStatusNote();
                        break;

                    case EquipmentStatus.Down:
                        equipmentStatusBackground.Fill = Brushes.Red;
                        OperationalStatusClick(offLineMenuItem, new RoutedEventArgs());
                        if (initComplete) AddEditEquipmentStatusNote();
                        break;

                    case EquipmentStatus.Scheduled:
                        equipmentStatusBackground.Fill = Brushes.Gray;
                        OperationalStatusClick(offLineMenuItem, new RoutedEventArgs());
                        break;
                }

            operationalMenuItem.IsChecked = degradedMenuItem.IsChecked = downMenuItem.IsChecked = 
                scheduleMenuItem.IsChecked = unscheduledMenuItem.IsChecked = false;

            item.IsChecked = true;

            downMenuItem.IsChecked = scheduleMenuItem.IsChecked || unscheduledMenuItem.IsChecked;
        }

        private void OperationalStatusClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item != null)
                slideTechHeader.Content = (OperationalStatus)item.Tag == OperationalStatus.OnLine ? "On Line" : "Off Line";

            onLineMenuItem.IsChecked = offLineMenuItem.IsChecked = false;
            item.IsChecked = true;
        }

        private void ScheduleOutageClick(object sender, RoutedEventArgs e)
        {
            var form = new NewScheduledOutageForm();

            if (form.ShowDialog() == true)
            {
                var outage = new EquipmentScheduledOutage(this, form.ScheduledOutage);
                ScheduledOutages.AddOutage(outage);
                scheduledOutages.Add(outage);
            }

        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (EquipmentDelete != null)
                EquipmentDelete.Invoke(this, new EquipmentDeleteEventArgs() { Equipment = this });
        }

        public override string ToString()
        {
            string output = "";

            output += "Start Equipment: " + adminEquipmentTextBox.Text + "\n";
            output += "Eqiup Status = " + status.ToString() + ", Oper Status = " + slideTechHeader.Content.ToString() + "\n";
            output += "Equip Status Note = " + noteText.Text + "\n";

            foreach (var outage in scheduledOutages)
                output += "Schedule Outage: Start Date = " + outage.Start.ToString("MMddyyyy") +
                    ", Start Time = " + outage.Start.ToString("HHmm") +
                    ", End Date = " + outage.End.ToString("MMddyyyy") +
                    ", End Time = " + outage.End.ToString("HHmm") +
                    ", Notes = " + outage.Notes + "\n";

            output += "End Equipment\n";

            return output;
        }
    }

    internal class EquipmentDeleteEventArgs : EventArgs
    {
        internal Equipment Equipment;
    }
}
