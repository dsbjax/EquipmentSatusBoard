using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.EquipmentControls;
using EquipmentSatusBoard.Forms;
using EquipmentSatusBoard.StatusBoardControl;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EquipmentSatusBoard.ScheduledOutageControls
{
    /// <summary>
    /// Interaction logic for EquipmentScheduledOutage.xaml
    /// </summary>
    public partial class EquipmentScheduledOutage : UserControl, IComparable<EquipmentScheduledOutage>, IAppMode
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private string statusNote;

        private DateTime start;
        public DateTime Start
        {
            get => start;
            set
            {
                start = value;
                startLabel.Content = value.ToUniversalTime().ToString("MM/dd HHmm");
            }
        }

        private DateTime end;
        public DateTime End
        {
            get => end;
            set
            {
                end = value;
                endLabel.Content = value.ToUniversalTime().ToString("MM/dd HHmm");
            }
        }

        public string Notes
        {
            get => (string)notes.Content;

            set
            {
                notes.Content = value;
            }
        }
        private Equipment equipment;
        private AppMode mode;

        public Equipment Equipment
        {
            get { return equipment; }
            set
            {
                equipment = value;
                equipmentLabel.Content = equipment.EquipmentName;
            }
        }

        public EquipmentScheduledOutage()
        {
            InitializeComponent();

            AppModeNotifications.Subscribe(this);
            StartTimer();
            mode = AppMode.Tech;
        }

        public EquipmentScheduledOutage(Equipment equipment, ScheduledOutage scheduledOutage)
        {
            InitializeComponent();

            Equipment = equipment;
            Start = scheduledOutage.OutageStart;
            End = scheduledOutage.OutageEnd;
            Notes = scheduledOutage.Notes;

            AppModeNotifications.Subscribe(this);
            StartTimer();
            mode = AppMode.Tech;
        }

        private void StartTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (background.Background != Brushes.LightBlue && DateTime.UtcNow.Date.CompareTo(start.Date) > 0)
                background.Background = Brushes.LightBlue;

            if (background.Background != Brushes.LightGreen && DateTime.UtcNow.Date.CompareTo(start.Date) == 0)
                background.Background = Brushes.LightGreen;

            if (background.Background != Brushes.Red && (DateTime.UtcNow.CompareTo(start) > -1 && DateTime.UtcNow.CompareTo(end) < 1))
            {
                background.Background = Brushes.Red;
                equipment.StartScheduledOutage();

                statusNote = equipment.EquipmentName + ": ";
                statusNote += Notes + " ";
                statusNote += (Start.Date.Equals(End.Date) ? Start.ToString("HHmm") : Start.ToString("MM/dd HHmm"))+ " to ";
                statusNote += Start.Date.Equals(End.Date) ? End.ToString("HHmm") : End.ToString("MM/dd HHmm");

                EquipmentStatusScrollingMarquee.AddEquipmentStatusText(statusNote);
                timer.Stop();
            }
        }

        public int CompareTo(EquipmentScheduledOutage other)
        {
            return start.CompareTo(other.start);
        }

        public void SetMode(AppMode newMode)
        {
            mode = newMode;
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            modifyOutage.IsEnabled = deleteOutage.IsEnabled = background.Background != Brushes.Red;
            endOutage.IsEnabled = background.Background == Brushes.Red;

            if (mode != AppMode.Slide)
                base.OnContextMenuOpening(e);
            else
                e.Handled = true;
        }

        private void ModifyOutageClick(object sender, RoutedEventArgs e)
        {
            var form = new NewScheduledOutageForm();

            form.ScheduledOutage = new ScheduledOutage()
            {
                OutageStart = Start,
                OutageEnd = End,
                Notes = Notes

            };

            if(form.ShowDialog() == true)
            {
                Start = form.ScheduledOutage.OutageStart;
                End = form.ScheduledOutage.OutageEnd;
                notes.Content = form.ScheduledOutage.Notes;
            }
        }

        private void DeleteOutageClick(object sender, RoutedEventArgs e)
        {
            ScheduledOutages.RemoveOutage(this);
            Equipment.RemoveScheduledOutage(this);
        }

        private void EndOutage_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            equipment.EndScheduledOutage(this);
            ScheduledOutages.RemoveOutage(this);
            EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(statusNote);
        }

        private void TimeTextboxGotFocus(object sender, RoutedEventArgs e)
        {
            ((TimeTextBox)sender).Text = "";
        }
    }

    public struct ScheduledOutage
    {
        public DateTime OutageStart, OutageEnd;
        public string Notes;
    }
}
