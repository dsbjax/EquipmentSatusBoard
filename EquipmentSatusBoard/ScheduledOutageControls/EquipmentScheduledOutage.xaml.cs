﻿using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.EquipmentControls;
using EquipmentSatusBoard.Forms;
using EquipmentSatusBoard.StatusBoardControl;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFLibrary;

namespace EquipmentSatusBoard.ScheduledOutageControls
{
    /// <summary>
    /// Interaction logic for EquipmentScheduledOutage.xaml
    /// </summary>
    public partial class EquipmentScheduledOutage : UserControl, IComparable<EquipmentScheduledOutage>, IAppMode, IAppTimer
    {
        //private DispatcherTimer timer = new DispatcherTimer();
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

            AppTimer.Subscribe(this, TimerInterval.Minutes);
            AppModeNotifications.Subscribe(this);
            mode = AppMode.Tech;
        }

        public EquipmentScheduledOutage(Equipment equipment, ScheduledOutage scheduledOutage)
        {
            InitializeComponent();

            Equipment = equipment;
            Start = scheduledOutage.OutageStart;
            End = scheduledOutage.OutageEnd;
            Notes = scheduledOutage.Notes;

            AppTimer.Subscribe(this, TimerInterval.Minutes);
            AppModeNotifications.Subscribe(this);
            mode = AppMode.Tech;
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
            var form = new NewScheduledOutageForm
            {
                ScheduledOutage = new ScheduledOutage()
                {
                    OutageStart = Start,
                    OutageEnd = End,
                    Notes = Notes

                }
            };

            if (form.ShowDialog() == true)
            {
                Start = form.ScheduledOutage.OutageStart;
                End = form.ScheduledOutage.OutageEnd;
                notes.Content = form.ScheduledOutage.Notes;
            }
        }

        private void DeleteOutageClick(object sender, RoutedEventArgs e)
        {
            AppTimer.Unsubscribe(this);
            ScheduledOutages.RemoveOutage(this);
            Equipment.RemoveScheduledOutage(this);
        }

        private void EndOutageClick(object sender, RoutedEventArgs e)
        {
            AppTimer.Unsubscribe(this);
            equipment.EndScheduledOutage(this);
            ScheduledOutages.RemoveOutage(this);
            EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(statusNote);
        }

        private void TimeTextboxGotFocus(object sender, RoutedEventArgs e)
        {
            ((TimeTextBox)sender).Text = "";
        }

        public void Tick(TimerInterval interval)
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
                statusNote += (Start.Date.Equals(End.Date) ? Start.ToString("HHmm") : Start.ToString("MM/dd HHmm")) + " to ";
                statusNote += Start.Date.Equals(End.Date) ? End.ToString("HHmm") : End.ToString("MM/dd HHmm");

                EquipmentStatusScrollingMarquee.AddEquipmentStatusText(statusNote);
                AppTimer.Unsubscribe(this);
            }

        }
    }

    public struct ScheduledOutage
    {
        public DateTime OutageStart, OutageEnd;
        public string Notes;
    }
}
