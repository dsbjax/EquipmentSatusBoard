using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.Forms;
using System;
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
        internal delegate void EquipmentDeleteEventHandle(object sender, EquipmentDeleteEventArgs e);
        internal event EquipmentDeleteEventHandle EquipmentDelete;

        internal delegate void ScheduledOutageRequestEventHandler(object sender, SchedluedOutageRequestEventArgs e);
        internal event ScheduledOutageRequestEventHandler EquipmentScheduledOutageRequest;

        public Equipment()
        {
            InitializeComponent();

            AppModeNotifications.Subscribe(this);
            SetMode(AppMode.Slide);

            adminEquipmentTextBox.GotFocus += AdminEquipmentTextBoxGotFocus;

            SetMenuItemTags();
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
                switch ((EquipmentStatus)item.Tag)
                {
                    case EquipmentStatus.Operational:
                        if (equipmentStatusBackground.Fill == Brushes.Gray)
                        {
                            var form = new EndScheduledMaintainanceDialog();
                            if ((bool)form.ShowDialog() == true)
                            {
                                //TODO: End the scheduled maint
                                break;
                            }

                            return;
                        }
                        equipmentStatusBackground.Fill = Brushes.Green;
                        break;

                    case EquipmentStatus.Degraded:
                        equipmentStatusBackground.Fill = Brushes.Orange;
                        break;

                    case EquipmentStatus.Down:
                        equipmentStatusBackground.Fill = Brushes.Red;
                        OperationalStatusClick(offLineMenuItem, new RoutedEventArgs());
                        break;

                    case EquipmentStatus.Scheduled:
                        equipmentStatusBackground.Fill = Brushes.Gray;
                        break;
                }

            operationalMenuItem.IsChecked = degradedMenuItem.IsChecked = downMenuItem.IsChecked = false;
            item.IsChecked = true;
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
            if (EquipmentScheduledOutageRequest != null)
                EquipmentScheduledOutageRequest.Invoke(this, new SchedluedOutageRequestEventArgs() { Equipment = this });
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (EquipmentDelete != null)
                EquipmentDelete.Invoke(this, new EquipmentDeleteEventArgs() { Equipment = this });
        }
    }

    internal class EquipmentDeleteEventArgs : EventArgs
    {
        internal Equipment Equipment;
    }

    internal class SchedluedOutageRequestEventArgs : EventArgs
    {
        internal Equipment Equipment;
    }
}
