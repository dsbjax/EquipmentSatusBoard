using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.EquipmentControls;
using Microsoft.Win32;
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

namespace EquipmentSatusBoard.StatusBoardControl
{
    /// <summary>
    /// Interaction logic for EquipmentStatusPageController.xaml
    /// </summary>
    public partial class EquipmentStatusPageController : UserControl, IAppMode
    {
        private static string CURRENT_STATUS_BOARD_FOLDER = 
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
            Properties.Settings.Default.AppDataFolder;

        private static string CURRENT_STATUS_BOARD_FILE = CURRENT_STATUS_BOARD_FOLDER + 
            Properties.Settings.Default.CurrentStatusBoardFilename;

        private string SAVED_STATUS_PAGES_FOLDER = 
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + 
            Properties.Settings.Default.SavedPagesFolder;

        int pageCount = 0, currentPage = 0;
        DispatcherTimer timer = new DispatcherTimer();

        public EquipmentStatusPageController()
        {
            InitializeComponent();
            AppModeNotifications.Subscribe(this);
            timer.Tick += TimerTick;

            SetMode(AppMode.Slide);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            NextPageClick(this, new RoutedEventArgs());
        }

        private void SetVisibility()
        {
            adminCurrentPage.Content = currentPage;
            adminPageCount.Content = pageCount;
            addGroup.Visibility = currentPage != 0 ? Visibility.Visible : Visibility.Collapsed;

            for (int i = 0; i < pages.Children.Count; i++)
                pages.Children[i].Visibility = i == (currentPage - 1) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void NextPageClick(object sender, RoutedEventArgs e)
        {
            if (currentPage == pageCount)
                currentPage = 1;
            else
                currentPage++;

            SetVisibility();
        }

        private void AddGroupClick(object sender, RoutedEventArgs e)
        {
            var group = new EquipmentGroup();
            group.EquipmentGroupDelete += GroupDelete;
            group.PageGroup = true;

            ((StackPanel)pages.Children[currentPage - 1]).Children.Add(group);
        }

        private void GroupDelete(object sender, EquipmentGroupDeleteEventArgs e)
        {
            ((StackPanel)pages.Children[currentPage - 1]).Children.Remove(e.EquipmentGroup);
        }

        private void SlideIntervalValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            timer.Interval = new TimeSpan(0,0, (int)slideInterval.Value);
        }

        public void SetMode(AppMode newMode)
        {
            adminPageControls.Visibility = newMode == AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;
            adminTechNextPage.Visibility = pageCount > 1 && newMode != AppMode.Slide ? Visibility.Visible : Visibility.Collapsed;

            if (newMode == AppMode.Slide && pageCount > 1)
            {
                timer.Interval = new TimeSpan(0, 0, 10);
                timer.Start();
            }
            else
                timer.Stop();
        }

        public void Save()
        {
            Save(CURRENT_STATUS_BOARD_FILE);
        }

        private void Save(string filename)
        {
            string output = "";

            foreach (var page in pages.Children)
            {
                output += "Page Start\n";
                foreach (var item in ((StackPanel)page).Children)
                    output += item.ToString();

                output += "Page End\n";
            }

            using (StreamWriter writer = new StreamWriter(filename))
                writer.Write(output);
        }

        private void NewPageClick(object sender, RoutedEventArgs e)
        {
            pages.Children.Add(new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Vertical
            });

            pageCount = currentPage = pages.Children.Count;

            SetVisibility();
        }

        private void LoadPageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Load Status Page";
            openFile.Filter = "Equipment Status Page|*.page|Equipment Status Board|*.status|All Files|*.*";
            openFile.InitialDirectory = SAVED_STATUS_PAGES_FOLDER;
            openFile.Multiselect = false;

            if(openFile.ShowDialog() == true)
                LoadStatusPages(openFile.FileName);
        }

        private void LoadStatusPages(string filename)
        {
            if (File.Exists(filename))
                using (StreamReader statusLines = new StreamReader(filename))
                {
                    string line;
                    while (!statusLines.EndOfStream)
                    {
                        line = statusLines.ReadLine();

                        if (line.StartsWith("Page Start"))
                        {
                            StackPanel panel = new StackPanel()
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Orientation = Orientation.Vertical
                            };

                            while ((line = statusLines.ReadLine()).StartsWith("Start Group:"))
                            {
                                var group = new EquipmentGroup(line.Remove(0, 13), statusLines);
                                group.EquipmentGroupDelete += GroupDelete;

                                panel.Children.Add(group);

                            }

                            foreach (var page in pages.Children)
                                ((StackPanel)page).Visibility = Visibility.Collapsed;

                            panel.Visibility = Visibility.Visible;
                            pages.Children.Add(panel);

                            adminCurrentPage.Content = adminPageCount.Content = pageCount = currentPage = pages.Children.Count;
                        }
                    }
                }

            if (pageCount > 0) currentPage = 1;
            SetVisibility();
        }

        private void SavePageClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.AddExtension = true;
            saveFile.DefaultExt = ".page";
            saveFile.Title = "Save Status Page";
            saveFile.InitialDirectory = SAVED_STATUS_PAGES_FOLDER;
            
            saveFile.OverwritePrompt = true;
            saveFile.Filter = "Equipment Status Page|*.page";

            if (saveFile.ShowDialog() == true)
                SaveStatusPage(pages.Children[currentPage - 1], saveFile.FileName);
        }

        private void SaveStatusPage(UIElement uIElement, string fileName)
        {
            string output = "";

            output += "Page Start\n";
            foreach (var item in ((StackPanel)uIElement).Children)
                output += item.ToString();

            output += "Page End\n";

            using (StreamWriter writer = new StreamWriter(fileName))
                writer.Write(output);
        }

        private void SaveAllPagesClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.AddExtension = true;
            saveFile.DefaultExt = ".status";
            saveFile.Title = "Save Equipment Status Board";
            saveFile.InitialDirectory = SAVED_STATUS_PAGES_FOLDER;
            saveFile.OverwritePrompt = true;
            saveFile.Filter = "Equipment Status Board|*.status";

            saveFile.ShowDialog();

            if(saveFile.FileName != "")
                Save(saveFile.FileName);
        }

        private void DeletePageClick(object sender, RoutedEventArgs e)
        {
            pages.Children.RemoveAt(currentPage - 1);
            if (currentPage > --pageCount)
                currentPage--;

            SetVisibility();
        }

        private void PageMenuSubmenuOpened(object sender, RoutedEventArgs e)
        {
            deletePage.IsEnabled = SavePage.IsEnabled = saveAllPages.IsEnabled = pageCount > 0;
        }

        private void SetBackgroundImageClick(object sender, RoutedEventArgs e)
        {
            var getImage = new OpenFileDialog();

            getImage.Title = "Get Background Image";
            getImage.ShowDialog();
            ImageSource image = new BitmapImage(new Uri(getImage.FileName));
            

            backgroundImage.Source = image;
        }

        public void LoadStatusPages()
        {
            LoadStatusPages(CURRENT_STATUS_BOARD_FILE);
        }
    }
}
