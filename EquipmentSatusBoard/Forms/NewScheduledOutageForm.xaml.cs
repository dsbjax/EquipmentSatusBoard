using EquipmentSatusBoard.ScheduledOutageControls;
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

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            schedlue.IsEnabled = endDate.SelectedDate.Value.CompareTo(startDate.SelectedDate.Value) == 0 &&
                endTime.Text.CompareTo("0000") >= 0 && startTime.Text.CompareTo("0000") >= 0 &&
                endTime.Text.CompareTo("2359") <= 0 && startTime.Text.CompareTo("2359") <= 0 &&
                endTime.Text.CompareTo(startTime.Text) > 0

                ||

                endDate.SelectedDate.Value.CompareTo(startDate.SelectedDate.Value) > 0 &&
                endTime.Text.CompareTo("0000") >= 0 && startTime.Text.CompareTo("0000") >= 0 &&
                endTime.Text.CompareTo("2359") <= 0 && startTime.Text.CompareTo("2359") <= 0;
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
                    int.Parse(startTime.Text) / 100, int.Parse(startTime.Text) % 100, 0, DateTimeKind.Utc),
                    OutageEnd = new DateTime(endDate.SelectedDate.Value.Year, endDate.SelectedDate.Value.Month, endDate.SelectedDate.Value.Day,
                    int.Parse(endTime.Text) / 100, int.Parse(endTime.Text) % 100, 0, DateTimeKind.Utc),
                    Notes = notes.Text
                };
            }

            set
            {
                startDate.SelectedDate = value.OutageStart;
                startTime.Text = value.OutageStart.ToString("HHmm");
                endDate.SelectedDate = value.OutageEnd;
                endTime.Text = value.OutageEnd.ToString("HHmm");
                notes.Text = value.Notes;
            }
        }
    }
}
