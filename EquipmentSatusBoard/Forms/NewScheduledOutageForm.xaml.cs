using EquipmentSatusBoard.ScheduledOutageControls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for NewScheduledOutageForm.xaml
    /// </summary>
    public partial class NewScheduledOutageForm : Window
    {
        public NewScheduledOutageForm()
        {
            InitializeComponent();

            startDate.SelectedDate = endDate.SelectedDate = DateTime.Now;

        }

        private void ScheduleClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void StartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate.SelectedDate = startDate.SelectedDate;
        }

        private void TimeTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ((TimeTextBox)sender).Text = "";
        }

        public ScheduledOutage ScheduledOutage
        {
            get
            {
                return new ScheduledOutage()
                {
                    OutageStart = new DateTime(startDate.SelectedDate.Value.Year, startDate.SelectedDate.Value.Month, startDate.SelectedDate.Value.Day,
                    int.Parse(startTime.Time) / 100, int.Parse(startTime.Time) % 100, 0, DateTimeKind.Utc),
                    OutageEnd = new DateTime(endDate.SelectedDate.Value.Year, endDate.SelectedDate.Value.Month, endDate.SelectedDate.Value.Day,
                    int.Parse(endTime.Time) / 100, int.Parse(endTime.Time) % 100, 0, DateTimeKind.Utc),
                    Notes = notes.Text
                };
            }

            set
            {
                startDate.SelectedDate = value.OutageStart;
                startTime.Time = value.OutageStart.ToString("HHmm");
                endDate.SelectedDate = value.OutageEnd;
                endTime.Time = value.OutageEnd.ToString("HHmm");
                notes.Text = value.Notes;
            }
        }
    }
}
