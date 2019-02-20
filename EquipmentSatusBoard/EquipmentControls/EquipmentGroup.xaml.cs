using EquipmentSatusBoard.AppModeControls;
using EquipmentSatusBoard.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

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

        bool pageGroup = false;
        public bool PageGroup {
            get { return pageGroup; }

            set
            {
                pageGroup = value;
                if(pageGroup)
                {
                    groupBox.BorderThickness = new Thickness(0);
                    adminHeader.FontSize = slideTechHeader.FontSize = 36;
                    adminHeader.FontWeight = slideTechHeader.FontWeight = FontWeights.Bold;
                    adminHeader.Effect = slideTechHeader.Effect = new DropShadowEffect()
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

            slideTechHeader.Content = adminHeader.Text = form.GroupName;
            for (int i = 0; i < form.EquipmentCount; i++)
                AddEquipmentClick(this, new RoutedEventArgs());

            AppModeNotifications.Subscribe(this);
            SetMode(AppMode.Admin);
        }

        public EquipmentGroup(string groupName, StreamReader statusLines)
        {
            InitializeComponent();

            slideTechHeader.Content = adminHeader.Text = groupName;
            var groupOptions = statusLines.ReadLine().Split(',');
            wrap = groupOptions[0].Contains("True");
            horizontal = groupOptions[1].Contains("True");
            PageGroup = groupOptions[2].Contains("True");

            if (horizontal)
                wrapGroupPanel.Orientation = Orientation.Horizontal;
            else
                wrapGroupPanel.Orientation = Orientation.Vertical;


            string line;
            while(!(line = statusLines.ReadLine()).Equals("End Group"))
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
            slideTechHeader.Content = adminHeader.Text;
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

            slideTechHeader.Visibility = newMode != AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;
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
