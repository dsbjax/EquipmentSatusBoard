using EquipmentSatusBoard.AppModeControls;
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

namespace EquipmentSatusBoard.EquipmentControls
{
    /// <summary>
    /// Interaction logic for EquipmentGroup.xaml
    /// </summary>
    public partial class EquipmentGroup : UserControl, IAppMode
    {
        internal delegate void EquipmentGroupDeleteEventHandler(object sender, EquipmentGroupDeleteEventArgs e);
        internal event EquipmentGroupDeleteEventHandler EquipmentGroupDelete;

        internal delegate void EquipmentScheduledOutageRequestEventHandler(object sender, SchedluedOutageRequestEventArgs e);
        internal event EquipmentScheduledOutageRequestEventHandler GroupEquipmentScheduledOutageRequest;

        private bool wrap = true, horizontal = true;


        public EquipmentGroup()
        {
            InitializeComponent();

            var form = new NewGroupDialog();
            form.ShowDialog();

            slideTechHeader.Content = adminHeader.Text = form.GroupName;
            for (int i = 0; i < form.EquipmentCount; i++)
                AdminAddEquipmentClick(this, new RoutedEventArgs());

            AppModeNotifications.Subscribe(this);
            SetMode(AppMode.Admin);
        }

        private void AdminDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (EquipmentGroupDelete != null)
                EquipmentGroupDelete.Invoke(this, new EquipmentGroupDeleteEventArgs() { EquipmentGroup = this });
        }

        private void AdminAddGroupButtonClick(object sender, RoutedEventArgs e)
        {
            var group = new EquipmentGroup();
            group.EquipmentGroupDelete += GroupDelete;

            if (wrap)
                wrapGroupPanel.Children.Add(group);
            else
                noWrapGroupPanel.Children.Add(group);
        }

        private void GroupDelete(object sender, EquipmentGroupDeleteEventArgs e)
        {
            if (wrap)
                wrapGroupPanel.Children.Remove(e.EquipmentGroup);
            else
                noWrapGroupPanel.Children.Remove(e.EquipmentGroup);

        }

        private void AdminAddEquipmentClick(object sender, RoutedEventArgs e)
        {
            if (wrap)
                AddEquipment(wrapGroupPanel);
            else
                AddEquipment(noWrapGroupPanel);
        }

        private void AddEquipment(Panel panel)
        {
            var equipment = new Equipment();
            equipment.EquipmentDelete += EquipmentDelete;
            equipment.EquipmentScheduledOutageRequest += EquipmentScheduledOutageRequest;
            equipment.SetMode(AppMode.Admin);

            panel.Children.Add(equipment);
        }

        private void EquipmentScheduledOutageRequest(object sender, SchedluedOutageRequestEventArgs e)
        {
            GroupEquipmentScheduledOutageRequest?.Invoke(sender, e);
        }

        private void EquipmentDelete(object sender, EquipmentDeleteEventArgs e)
        {
            if (wrap)
                wrapGroupPanel.Children.Remove(e.Equipment);
            else
                noWrapGroupPanel.Children.Remove(e.Equipment);
        }

        private void AdminWrapButtonClick(object sender, RoutedEventArgs e)
        {
            wrap = !wrap;
            List<UIElement> elements = new List<UIElement>();

            if(wrap)
            {
                foreach (UIElement item in noWrapGroupPanel.Children)
                    elements.Add(item);

                noWrapGroupPanel.Children.Clear();

                foreach (UIElement item in elements)
                    wrapGroupPanel.Children.Add(item);

                adminWrapButton.Content = "No Wrap";
            }
            else
            {
                foreach (UIElement item in wrapGroupPanel.Children)
                    elements.Add(item);

                wrapGroupPanel.Children.Clear();

                foreach (UIElement item in elements)
                    noWrapGroupPanel.Children.Add(item);

                adminWrapButton.Content = "Wrap";
            }
        }

        private void AdminOrientationButtonClick(object sender, RoutedEventArgs e)
        {
            horizontal = !horizontal;

            if (horizontal)
                wrapGroupPanel.Orientation = noWrapGroupPanel.Orientation = Orientation.Horizontal;
            else
                wrapGroupPanel.Orientation = noWrapGroupPanel.Orientation = Orientation.Vertical;
        }

        private void AdminHeaderTextChanged(object sender, TextChangedEventArgs e)
        {
            slideTechHeader.Content = adminHeader.Text;
        }

        public void SetMode(AppMode newMode)
        {
            adminHeader.Visibility = adminDeleteButton.Visibility =
                newMode == AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;

            /*= adminAddGroupButton.Visibility = adminOrientationButton.Visibility = adminWrapButton.Visibility =
                adminAddEquipmentButton.Visibility = adminOrientationButton.Visibility*/

            slideTechHeader.Visibility = newMode != AppMode.Admin ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    internal class EquipmentGroupDeleteEventArgs : EventArgs
    {
        internal EquipmentGroup EquipmentGroup;
    }
}
