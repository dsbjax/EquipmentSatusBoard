﻿using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.EquipmentControls;
using EquipmentSatusBoard.Forms;
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
            if (DateTime.UtcNow.Date.CompareTo(start.Date) > 0)
                background.Fill = Brushes.LightBlue;

            if (DateTime.UtcNow.Date.CompareTo(start.Date) == 0)
                background.Fill = Brushes.LightGreen;

            if (DateTime.UtcNow.CompareTo(start) > -1 && DateTime.UtcNow.CompareTo(end) < 1)
            {
                background.Fill = Brushes.Red;
                equipment.StartScheduledOutage();
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
            modifyOutage.IsEnabled = deleteOutage.IsEnabled = background.Fill != Brushes.Red;
            endOutage.IsEnabled = background.Fill == Brushes.Red;

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
            Equipment.RemoveOutage(this);
        }

        private void EndOutage_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            equipment.EndScheduledOutage();
            ScheduledOutages.RemoveOutage(this);
            equipment.RemoveOutage(this);
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