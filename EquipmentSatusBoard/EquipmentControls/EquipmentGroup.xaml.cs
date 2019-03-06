using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.Forms;
using EquipmentSatusBoard.StatusBoardControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using WPFLibrary;

namespace EquipmentSatusBoard.EquipmentControls
{
    /// <summary>
    /// Interaction logic for EquipmentGroup.xaml
    /// </summary>
    public partial class EquipmentGroup : UserControl, IAppMode
    {
        internal delegate void EquipmentGroupDeleteEventHandler(object sender, EquipmentGroupDeleteEventArgs e);
        internal event EquipmentGroupDeleteEventHandler EquipmentGroupDelete;

        private bool wrap = true, horizontal = true;
        private string groupEquipmentStatus = null;

        bool pageGroup = false;
        public bool PageGroup {
            get { return pageGroup; }

            set
            {
                pageGroup = value;
                if(pageGroup)
                {
                    groupBox.BorderThickness = new Thickness(0);
                    adminHeader.FontSize = slideHeader.FontSize = techHeader.FontSize = 36;
                    adminHeader.FontWeight = slideHeader.FontWeight = techHeader.FontWeight = FontWeights.Bold;
                    adminHeader.Effect = slideHeader.Effect = techHeader.Effect = new DropShadowEffect()
                    {
                        BlurRadius = 5,
                        Color = Colors.Black,
                        Direction = 315,
                        Opacity = 100,
                        RenderingBias = RenderingBias.Performance,
                        ShadowDepth = 5
                    };
                }
            }
        }

        public EquipmentGroup()
        {
            InitializeComponent();

            var form = new NewGroupDialog();
            form.ShowDialog();

            slideHeader.Content = techHeader.Content = adminHeader.Text = form.GroupName;
            for (int i = 0; i < form.EquipmentCount; i++)
                AddEquipmentClick(this, new RoutedEventArgs());

            AppModeNotifications.Subscribe(this);
            SetMode(AppMode.Admin);
            SetTags();
        }

        public EquipmentGroup(string groupName, StreamReader statusLines)
        {
            InitializeComponent();

            try
            {
                slideHeader.Content = techHeader.Content = adminHeader.Text = groupName;
                var groupOptions = statusLines.ReadLine().Split(',');
                wrap = groupOptions[0].Contains("True");
                horizontal = groupOptions[1].Contains("True");
                PageGroup = groupOptions[2].Contains("True");

                if (horizontal)
                    wrapGroupPanel.Orientation = Orientation.Horizontal;
                else
                    wrapGroupPanel.Orientation = Orientation.Vertical;


                string line;

                while (!(line = statusLines.ReadLine()).Equals("End Group"))
                {
                    if (line.StartsWith("Start Group:"))
                    {
                        var group = new EquipmentGroup(line.Remove(0, 13), statusLines);
                        group.EquipmentGroupDelete += GroupDelete;

                        if (wrap)
                            wrapGroupPanel.Children.Add(group);
                        else
                            noWrapGroupPanel.Children.Add(group);
                    }
                    else
                    {
                        var equipment = new Equipment(line.Remove(0, 17), statusLines);
                        equipment.EquipmentDelete += EquipmentDelete;

                        if (wrap)
                            wrapGroupPanel.Children.Add(equipment);
                        else
                            noWrapGroupPanel.Children.Add(equipment);
                    }
                }

                AppModeNotifications.Subscribe(this);
                SetMode(AppMode.Slide);
                SetTags();

            }catch(OutOfMemoryException e)
            {
                var error = "Error Importing Equipment Group:\nAn Out of Memory Exception occured\nSee dump file";

                ErrorLogger.ErrorDialog(error, ErrorType.Failure);
                ErrorLogger.LogError(error, e,
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                    Properties.Settings.Default.AppDataFolder +
                    Properties.Settings.Default.ErrorLogFilename);

            }
            catch (IOException e)
            {
                var error = "Error Importing Equipment Group:\nAn IO Exception occured\nSee dump file";

                ErrorLogger.ErrorDialog(error, ErrorType.Failure);
                ErrorLogger.LogError(error, e,
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                    Properties.Settings.Default.AppDataFolder +
                    Properties.Settings.Default.ErrorLogFilename);

            }
            catch (Exception e)
            {
                var error = "Error Importing Equipment Group:\nAn Unknown Exception occured\nSee dump file";

                ErrorLogger.ErrorDialog(error, ErrorType.Failure);
                ErrorLogger.LogError(error, e,
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                    Properties.Settings.Default.AppDataFolder +
                    Properties.Settings.Default.ErrorLogFilename);

            }
        }

        private void SetTags()
        {
            operationalMenuItem.Tag = EquipmentStatus.Operational;
            degradedMenuItem.Tag = EquipmentStatus.Degraded;
            unscheduledMenuItem.Tag = EquipmentStatus.Down;
        }

        private void AdminDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (EquipmentGroupDelete != null)
            {
                foreach (var item in wrapGroupPanel.Children)
                    AppModeNotifications.Unsubscribe((IAppMode)item);

                foreach (var item in noWrapGroupPanel.Children)
                    AppModeNotifications.Unsubscribe((IAppMode)item);

                wrapGroupPanel.Children.Clear();
                noWrapGroupPanel.Children.Clear();
                AppModeNotifications.Unsubscribe(this);

                EquipmentGroupDelete.Invoke(this, new EquipmentGroupDeleteEventArgs() { EquipmentGroup = this });
            }
        }

        private void GroupDelete(object sender, EquipmentGroupDeleteEventArgs e)
        {
            if (wrap)
                wrapGroupPanel.Children.Remove(e.EquipmentGroup);
            else
                noWrapGroupPanel.Children.Remove(e.EquipmentGroup);

        }

        private void AddEquipment(Panel panel)
        {
            var equipment = new Equipment();
            equipment.EquipmentDelete += EquipmentDelete;
            equipment.SetMode(AppMode.Admin);

            panel.Children.Add(equipment);
        }

        private void EquipmentDelete(object sender, EquipmentDeleteEventArgs e)
        {
            if (wrap)
                wrapGroupPanel.Children.Remove(e.Equipment);
            else
                noWrapGroupPanel.Children.Remove(e.Equipment);
        }

        private void AdminHeaderTextChanged(object sender, TextChangedEventArgs e)
        {
            slideHeader.Content = techHeader.Content = adminHeader.Text;
        }

        private void AddEquipmentGroupClick(object sender, RoutedEventArgs e)
        {
            var group = new EquipmentGroup();
            group.EquipmentGroupDelete += GroupDelete;

            if (wrap)
                wrapGroupPanel.Children.Add(group);
            else
                noWrapGroupPanel.Children.Add(group);
        }

        private void AddEquipmentClick(object sender, RoutedEventArgs e)
        {
            if (wrap)
                AddEquipment(wrapGroupPanel);
            else
                AddEquipment(noWrapGroupPanel);
        }

        private void IsWrapClick(object sender, RoutedEventArgs e)
        {
            wrap = !wrap;
            isWrap.IsChecked = wrap;

            List<UIElement> elements = new List<UIElement>();

            if (wrap)
            {
                foreach (UIElement item in noWrapGroupPanel.Children)
                    elements.Add(item);

                noWrapGroupPanel.Children.Clear();

                foreach (UIElement item in elements)
                    wrapGroupPanel.Children.Add(item);
            }
            else
            {
                foreach (UIElement item in wrapGroupPanel.Children)
                    elements.Add(item);

                wrapGroupPanel.Children.Clear();

                foreach (UIElement item in elements)
                    noWrapGroupPanel.Children.Add(item);
            }
        }

        private void IsHorizontalClick(object sender, RoutedEventArgs e)
        {
            horizontal = !horizontal;
            isHorizontal.IsChecked = horizontal;

            if (horizontal)
                wrapGroupPanel.Orientation = Orientation.Horizontal;
            else
                wrapGroupPanel.Orientation = Orientation.Vertical;
        }

        public void SetMode(AppMode newMode)
        {
            adminHeader.Visibility = adminDeleteButton.Visibility = adminMenu.Visibility =
                newMode == AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;

            slideHeader.Visibility = newMode == AppMode.Slide ? Visibility.Visible : Visibility.Collapsed;
            techHeader.Visibility = newMode == AppMode.Tech ? Visibility.Visible : Visibility.Collapsed;
        }

        public override string ToString()
        {
            string output = "";

            output += "Start Group: " + adminHeader.Text + "\n";
            output += "Wrap = " + wrap.ToString() + ", Horizontal = " + horizontal.ToString() + ", Page Group = " + pageGroup.ToString() + "\n";

            foreach (var item in ActivePanel())
                output += item.ToString();

            output += "End Group\n";

            return output;
        }

        private void HideClick(object sender, RoutedEventArgs e)
        {
            if (wrap)
                wrapGroupPanel.Visibility = wrapGroupPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            else
                noWrapGroupPanel.Visibility = noWrapGroupPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AdminMenu_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            isWrap.IsChecked = wrap;
            isHorizontal.IsChecked = horizontal;
        }

        private void GroupEquipmentStatusClick(object sender, RoutedEventArgs e)
        {
            Panel activePanel;
            var newStatus = (EquipmentStatus)((MenuItem)sender).Tag;

            if (wrapGroupPanel.Children.Count > 0)
                activePanel = wrapGroupPanel;
            else
                activePanel = noWrapGroupPanel;

            foreach (var element in activePanel.Children)
                if ( element.GetType() == typeof(Equipment))
                    ((Equipment)element).GroupEquipmentStatusChange(newStatus);

            if (newStatus == EquipmentStatus.Operational)
                EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(adminHeader.Text + ": " + groupEquipmentStatus);
            else
            {
                var form = new EquipmentStatusNoteForm();
                form.ShowDialog();

                if (form.Note != null)
                {
                        if (groupEquipmentStatus != null)
                            EquipmentStatusScrollingMarquee.RemoveEquipmentStatusText(adminHeader.Text + ": " + groupEquipmentStatus);

                        EquipmentStatusScrollingMarquee.AddEquipmentStatusText(adminHeader.Text + ": " + form.Note);
                        groupEquipmentStatus = form.Note;
                }
            }

            operationalMenuItem.IsChecked = newStatus == EquipmentStatus.Operational;
            degradedMenuItem.IsChecked = newStatus == EquipmentStatus.Degraded;
            unscheduledMenuItem.IsChecked = newStatus == EquipmentStatus.Down;
            
        }

        private UIElementCollection ActivePanel()
        {
            return wrap ? wrapGroupPanel.Children : noWrapGroupPanel.Children;
        }
    }

    internal class EquipmentGroupDeleteEventArgs : EventArgs
    {
        internal EquipmentGroup EquipmentGroup;
    }
}
