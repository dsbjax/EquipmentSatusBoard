using System.Collections.Generic;
using System.Windows.Controls;

namespace EquipmentSatusBoard.Forms
{
    /// <summary>
    /// Interaction logic for SelectTimeDropList.xaml
    /// </summary>
    public partial class SelectTimeDropList : UserControl
    {
        public SelectTimeDropList()
        {
            InitializeComponent();

            List<string> times = new List<string>();
            for (var sec = 0; sec < 60; sec++)
                for (var hour = 0; hour < 24; hour++)
                    times.Add(hour.ToString().PadLeft(2, '0') + sec.ToString().PadLeft(2, '0'));

            times.Sort();

            foreach (var time in times)
                selectTime.Items.Add(time);
        }

        internal string Time {
            get
            { return selectTime.SelectedValue.ToString(); }

            set
            {
                selectTime.SelectedItem = value;
            }
        }
    }
}
