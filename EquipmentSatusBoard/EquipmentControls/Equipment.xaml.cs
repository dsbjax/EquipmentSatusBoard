using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.CommonControls;
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
        private const string TAG_DELIMITER = ": ";

        private List<EquipmentScheduledOutage> scheduledOutages = new List<EquipmentScheduledOutage>();
        private EquipmentStatus status;
        private bool initComplete = false;

        public object EquipmentName { get { return slideEquipmentLabel.Content; } }

        internal delegate void EquipmentDeleteEventHandle(object sender, EquipmentDeleteEventArgs e);
        internal event EquipmentDeleteEventHandle EquipmentDelete;

        #region Constructors
        public Equipment()
        {
            InitializerCommonTasks();

            equipmentStatusBackground.ToolTipOpening += EquipmentStatusBackground_ToolTipOpening;

            initComplete = true;
        }

        private void EquipmentStatusBackground_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (equipmentStatusBackground.ToolTip.ToString().Length == 0) e.Handled = true;
        }

        //Used when importing equipment from a status file
        public Equipment(string equipmentName, StreamReader statusLines)
        {
            InitializerCommonTasks();

            try
            {
                //Set equipment name on the controls used in different modes
                slideEquipmentLabel.Content = techEquipmentLabel.Content = adminEquipmentTextBox.Text = equipmentName;

                //Read in the equipment optios line and set options
                var equipmentOptions = statusLines.ReadLine().Split(',');
                SetImportedEquipmentStatus(equipmentOptions);
                SetImportedOperationalStatus(equipmentOptions);
                SetImportedEquipmentNotes(statusLines);
                SetImportedEquipmentScheduledOutages(statusLines);

                initComplete = true;

            }catch(Exception ex)
            {
                ErrorLogger.LogError("Error Initializing Equipment from Status File, Equipment:Equipment()", ex);
            }
        }

        // Equipment Initializers Start
        private void InitializerCommonTasks()
        {
            InitializeComponent();

            AppModeNotifications.Subscribe(this);
            SetMode(AppMode.Slide);

            adminEquipmentTextBox.GotFocus += AdminEquipmentTextBoxSelectAll;

            SetMenuItemTags();
        }

        private void SetImportedEquipmentScheduledOutages(StreamReader statusLines)
        {
            string line;
            while (!(line = statusLines.ReadLine()).Equals("End Equipment"))
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
        }

        private void SetImportedEquipmentNotes(StreamReader statusLines)
        {
            equipmentStatusBackground.ToolTip = statusLines.ReadLine().Replace(Properties.Settings.Default.EquipStatsNoteHeader, null);
            if (equipmentStatusBackground.ToolTip.ToString() != "")
                EquipmentStatusScrollingMarquee.AddEquipmentStatusText(EquipmentName + ": " + equipmentStatusBackground.ToolTip.ToString());
        }

        private void SetImportedOperationalStatus(string[] equipmentOptions)
        {
            if (equipmentOptions[1].Contains("On"))
                OperationalStatusClick(onLineMenuItem, new RoutedEventArgs());
            else
                OperationalStatusClick(offLineMenuItem, new RoutedEventArgs());
        }

        private void SetImportedEquipmentStatus(string[] equipmentOptions)
        {
            switch (equipmentOptions[0].Substring(equipmentOptions[0].LastIndexOf(' ') + 1))
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
        #endregion

        private void AddEditEquipmentStatusNote()
        {
            var form = new EquipmentStatusNoteForm();
            form.ShowDialog();

            if (form.Note != null)
            {
                if (equipmentStatusBackground.ToolTip.ToString() != null)
                    EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(EquipmentName + TAG_DELIMITER + equipmentStatusBackground.ToolTip.ToString());

                EquipmentStatusScrollingMarquee.AddEquipmentStatusText(EquipmentName + TAG_DELIMITER + form.Note);
                equipmentStatusBackground.ToolTip = form.Note;
            }
        }

        internal void EndScheduledOutage(EquipmentScheduledOutage outage)
        {
            EquipmentStatusClick(operationalMenuItem, new RoutedEventArgs());
            OperationalStatusClick(onLineMenuItem, new RoutedEventArgs());
            RemoveScheduledOutage(outage);
        }

        internal void RemoveScheduledOutage(EquipmentScheduledOutage outage)
        {
            scheduledOutages.Remove(outage);
        }

        internal void StartScheduledOutage()
        {
            EquipmentStatusClick(scheduleMenuItem, new RoutedEventArgs());
        }

        private void AdminEquipmentTextBoxSelectAll(object sender, RoutedEventArgs e)
        {
            adminEquipmentTextBox.SelectAll();
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
            try
            {
                MenuItem item = (MenuItem)sender;
                if (item != null)
                    switch (status = (EquipmentStatus)item.Tag)
                    {
                        case EquipmentStatus.Operational:
                            equipmentStatusBackground.Fill = Brushes.Green;
                            if (initComplete && equipmentStatusBackground.ToolTip.ToString() != "")
                                EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(EquipmentName + ": " + equipmentStatusBackground.ToolTip.ToString());

                            equipmentStatusBackground.ToolTip = "";
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

            }catch(Exception ex)
            {
                ErrorLogger.LogError("Error Setting Operational Status, Equipment:Equipment StatusCLick()", ex);
            }
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
            {
                AppModeNotifications.Unsubscribe(this);
                EquipmentDelete.Invoke(this, new EquipmentDeleteEventArgs() { Equipment = this });
            }
        }

        public override string ToString()
        {
            string output = "";

            output += "Start Equipment: " + adminEquipmentTextBox.Text + "\n";
            output += "Eqiup Status = " + status.ToString() + ", Oper Status = " + slideTechHeader.Content.ToString() + "\n";
            output += "Equip Status Note = " + equipmentStatusBackground.ToolTip.ToString() + "\n";

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
