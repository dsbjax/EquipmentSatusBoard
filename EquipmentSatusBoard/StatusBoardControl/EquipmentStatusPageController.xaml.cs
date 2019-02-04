using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.EquipmentControls;
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

namespace EquipmentSatusBoard.StatusBoardControl
{
    /// <summary>
    /// Interaction logic for EquipmentStatusPageController.xaml
    /// </summary>
    public partial class EquipmentStatusPageController : UserControl, IAppMode
    {
        int pageCount = 0, currentPage = 0;
        DispatcherTimer timer = new DispatcherTimer();

        public EquipmentStatusPageController()
        {
            InitializeComponent();
            AppModeNotifications.Subscribe(this);
            timer.Tick += Timer_Tick;

            SetMode(AppMode.Slide);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            NextPage_Click(this, new RoutedEventArgs());
        }

        private void AddPage_Click(object sender, RoutedEventArgs e)
        {
            pages.Children.Add(new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical
            });

            pageCount = currentPage = pages.Children.Count;

            SetVisibility();
        }

        private void SetVisibility()
        {
            adminCurrentPage.Content = currentPage;
            adminPageCount.Content = pageCount;
            deletePage.Visibility = pageCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            nextPage.Visibility = pageCount > 1 ? Visibility.Visible : Visibility.Collapsed;
            addGroup.Visibility = currentPage != 0 ? Visibility.Visible : Visibility.Collapsed;

            for (int i = 0; i < pages.Children.Count; i++)
                pages.Children[i].Visibility = i == (currentPage - 1) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DeletePage_Click(object sender, RoutedEventArgs e)
        {
            pages.Children.RemoveAt(currentPage - 1);
            if (currentPage > --pageCount)
                currentPage--;

            SetVisibility();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == pageCount)
                currentPage = 1;
            else
                currentPage++;

            SetVisibility();
        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            var group = new EquipmentGroup();
            group.EquipmentGroupDelete += GroupDelete;
            ((StackPanel)pages.Children[currentPage - 1]).Children.Add(group);
        }

        private void GroupDelete(object sender, EquipmentGroupDeleteEventArgs e)
        {
            ((StackPanel)pages.Children[currentPage - 1]).Children.Remove(e.EquipmentGroup);
        }

        public void SetMode(AppMode newMode)
        {
            adminPageControls.Visibility = newMode == AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;

            if (newMode == AppMode.Slide && pageCount > 1)
            {
                timer.Interval = new TimeSpan(0, 0, 10);
                timer.Start();
            }
            else
                timer.Stop();
        }
    }
}
