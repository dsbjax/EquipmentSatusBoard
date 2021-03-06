﻿using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.EquipmentControls;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFLibrary;

namespace EquipmentSatusBoard.StatusBoardControl
{
    /// <summary>
    /// Interaction logic for EquipmentStatusPageController.xaml
    /// </summary>
    public partial class EquipmentStatusPageController : UserControl, IAppMode, IAppTimer
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

        public EquipmentStatusPageController()
        {
            InitializeComponent();
            AppModeNotifications.Subscribe(this);

            LoadStatusPages();
        }

        private void TimerTick(object sender, EventArgs e)
        {
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
            AppTimer.Unsubscribe(this);
            AppTimer.Subscribe(this, TimerInterval.Seconds, (int)slideInterval.Value);
        }

        public void SetMode(AppMode newMode)
        {
            adminPageControls.Visibility = newMode == AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;
            adminTechNextPage.Visibility = pageCount > 1 && newMode != AppMode.Slide ? Visibility.Visible : Visibility.Collapsed;

            if (newMode == AppMode.Slide && pageCount > 1)
            {
                SlideIntervalValueChanged(this, null);
            }
            else
                AppTimer.Unsubscribe(this);

            if (newMode == AppMode.Slide)
                Save();
        }

        public void Save()
        {
            Save(CURRENT_STATUS_BOARD_FILE);
        }

        private void Save(string filename)
        {
            string output = "";

            foreach (var page in pages.Children)
                output += page.ToString();

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
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Load Status Page",
                Filter = "Equipment Status Page|*.page|Equipment Status Board|*.status|All Files|*.*",
                InitialDirectory = SAVED_STATUS_PAGES_FOLDER,
                Multiselect = false
            };

            if (openFile.ShowDialog() == true)
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

                        try
                        {
                            if (line.StartsWith("Page Start"))
                            {
                                var statusPage = new StatusPage(statusLines);

                                foreach (var page in pages.Children)
                                    ((StatusPage)page).Visibility = Visibility.Collapsed;

                                statusPage.Visibility = Visibility.Visible;
                                pages.Children.Add(statusPage);

                                adminCurrentPage.Content = adminPageCount.Content = pageCount = currentPage = pages.Children.Count;
                            }
                        }catch(Exception e)
                        {
                            var error = "Error Loading Status pages";

                            ErrorLogger.ErrorDialog(error, ErrorType.Failure);
                            ErrorLogger.LogError(error, e,
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                    Properties.Settings.Default.AppDataFolder +
                    Properties.Settings.Default.ErrorLogFilename);

                        }
                    }
                }

            if (pageCount > 0) currentPage = 1;
            SetVisibility();
        }

        private void SavePageClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".page",
                Title = "Save Status Page",
                InitialDirectory = SAVED_STATUS_PAGES_FOLDER,

                OverwritePrompt = true,
                Filter = "Equipment Status Page|*.page"
            };

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
            SaveFileDialog saveFile = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".status",
                Title = "Save Equipment Status Board",
                InitialDirectory = SAVED_STATUS_PAGES_FOLDER,
                OverwritePrompt = true,
                Filter = "Equipment Status Board|*.status"
            };

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
            deleteBackgroundImage.IsEnabled = deletePage.IsEnabled && 
                ((StatusPage)pages.Children[currentPage - 1]).HasBackgroundImage;
        }

        private void SetBackgroundImageClick(object sender, RoutedEventArgs e)
        {
            var getImage = new OpenFileDialog
            {
                Title = "Get Background Image"
            };
            getImage.ShowDialog();

            ((StatusPage)pages.Children[currentPage - 1]).SetBackgroundImage(getImage.FileName);
        }

        private void DeleteBackgroundImageClick(object sender, RoutedEventArgs e)
        {
            ((StatusPage)pages.Children[currentPage - 1]).SetBackgroundImage(null);
        }

        public void LoadStatusPages()
        {
            LoadStatusPages(CURRENT_STATUS_BOARD_FILE);
        }

        public void Tick(TimerInterval interval)
        {
            NextPageClick(this, new RoutedEventArgs());
        }
    }
}
