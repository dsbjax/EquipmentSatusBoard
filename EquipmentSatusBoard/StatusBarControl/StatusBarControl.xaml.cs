using EquipmentSatusBoard.AppModeControls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFLibrary;

namespace EquipmentSatusBoard.StatusBarControl
{
    /// <summary>
    /// Interaction logic for StatusBarControl.xaml
    /// </summary>
    public partial class StatusBarControl : UserControl, IAppMode, IAppTimer
    {
        private static string PHONE_NUMBERS_FOLDER = 
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            + Properties.Settings.Default.AppDataFolder;

        private static string PHONE_NUMBERS_FILE = PHONE_NUMBERS_FOLDER + Properties.Settings.Default.SavedPhoneNumbersFilename;

        internal delegate void AppModeChangeRequestEventHandler(object sender, EventArgs e);
        internal event AppModeChangeRequestEventHandler AppModeChangeRequest;

        public StatusBarControl()
        {
            InitializeComponent();

            SetDateTime();

            AppTimer.Subscribe(this, TimerInterval.Minutes);
            AppModeNotifications.Subscribe(this);
        }

        public void SetMode(AppMode newMode)
        {
            if(newMode == AppMode.Admin)
            {
                slideCDO.Visibility = slideDutyTech.Visibility = Visibility.Collapsed;
                adminCDO.Visibility = adminDutyTech.Visibility = Visibility.Visible;
            }
            else
            {
                slideCDO.Visibility = slideDutyTech.Visibility = Visibility.Visible;
                adminCDO.Visibility = adminDutyTech.Visibility = Visibility.Collapsed;
            }

            modeChangeRequest.Content = "Mode: " + newMode.ToString();

            if (newMode == AppMode.Slide)
                SavePhoneNumbers();
        }

        private void ModeChangeRequestClick(object sender, RoutedEventArgs e)
        {
            if (AppModeChangeRequest != null)
                AppModeChangeRequest.Invoke(this, new EventArgs());

        }

        private void AdminDutyTechTextChanged(object sender, TextChangedEventArgs e)
        {
            slideDutyTech.Content = adminDutyTech.Text;
        }

        private void AdminCDOTextChanged(object sender, TextChangedEventArgs e)
        {
            slideCDO.Content = adminCDO.Text;
        }

        public override void EndInit()
        {
            base.EndInit();

            LoadPhoneNumbers();
        }

        private void LoadPhoneNumbers()
        {
            if(File.Exists(PHONE_NUMBERS_FILE))
                using (StreamReader reader = new StreamReader(PHONE_NUMBERS_FILE))
                {
                    slideCDO.Content = adminCDO.Text = reader.ReadLine();
                    slideDutyTech.Content = adminDutyTech.Text = reader.ReadLine();
                }
        }

        internal void SavePhoneNumbers()
        {
            using (StreamWriter writer = new StreamWriter(PHONE_NUMBERS_FILE))
            {
                writer.WriteLine(adminCDO.Text);
                writer.WriteLine(adminDutyTech.Text);
            }
        }

        public void Tick(TimerInterval interval)
        {
            SetDateTime();
        }

        private void SetDateTime()
        {
            date.Content = DateTime.Now.ToLongDateString();
            local.Content = DateTime.Now.ToString("HHmm");
            utc.Content = DateTime.UtcNow.ToString("HHmm");
        }
    }
}
