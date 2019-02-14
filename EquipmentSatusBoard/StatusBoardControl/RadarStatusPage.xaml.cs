using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.EquipmentControls;
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

namespace EquipmentSatusBoard.StatusBoardControl
{
    /// <summary>
    /// Interaction logic for RadarStatusPage.xaml
    /// </summary>
    public partial class RadarStatusPage : UserControl, IAppMode
    {
        int radarCount = 0;

        public RadarStatusPage()
        {
            InitializeComponent();
            AppModeNotifications.Subscribe(this);

            LoadRadars();
        }

        private void LoadRadars()
        {
            if(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Properties.Settings.Default.AppDataFolder + Properties.Settings.Default.SavedRadarsFilename))
                using (var reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Properties.Settings.Default.AppDataFolder + Properties.Settings.Default.SavedRadarsFilename))
                {
                    string line;
                    while(!reader.EndOfStream && (line = reader.ReadLine()).StartsWith("Start Equipment:"))
                    {
                        var radar = new Equipment(line.Remove(0, 17), reader);
                        radar.EquipmentDelete += RadarDelete;
                        radar.Width = Double.NaN;

                        radars.Children.Add(radar);
                    }
                }
        }

        public void SetMode(AppMode newMode)
        {
            addRadar.Visibility = newMode == AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddRadar_Click(object sender, RoutedEventArgs e)
        {
            var newRadar = new Equipment();
            newRadar.EquipmentDelete += RadarDelete;
            newRadar.SetMode(AppMode.Admin);
            newRadar.Width = Double.NaN;

            radars.Children.Add(newRadar);

            addRadar.Visibility = ++radarCount < 10 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RadarDelete(object sender, EquipmentDeleteEventArgs e)
        {
            radars.Children.Remove(e.Equipment);
            addRadar.Visibility = ++radarCount < 10 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Save()
        {

            using (var writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Properties.Settings.Default.AppDataFolder + Properties.Settings.Default.SavedRadarsFilename))
            {
                foreach (var radar in radars.Children)
                    writer.Write(radar.ToString());
            }
        }
    }
}
