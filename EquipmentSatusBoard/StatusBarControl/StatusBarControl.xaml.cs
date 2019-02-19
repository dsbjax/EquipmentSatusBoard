using EquipmentSatusBoard.AppModeControls;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace EquipmentSatusBoard.StatusBarControl
{
    /// <summary>
    /// Interaction logic for StatusBarControl.xaml
    /// </summary>
    public partial class StatusBarControl : UserControl, IAppMode
    {
        private static string PHONE_NUMBERS_FOLDER = 
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            + Properties.Settings.Default.AppDataFolder;

        private static string PHONE_NUMBERS_FILE = PHONE_NUMBERS_FOLDER + Properties.Settings.Default.SavedPhoneNumbersFilename;

        internal delegate void AppModeChangeRequestEventHandler(object sender, EventArgs e);
        internal event AppModeChangeRequestEventHandler AppModeChangeRequest;

        DispatcherTimer timer = new DispatcherTimer();

        public StatusBarControl()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += TimerTick;
            timer.Start();

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

        private void TimerTick(object sender, EventArgs e)
        {
            date.Content = DateTime.Now.ToLongDateString();
            local.Content = DateTime.Now.ToString("HHmm");
            utc.Content = DateTime.UtcNow.ToString("HHmm");
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
    }
}
