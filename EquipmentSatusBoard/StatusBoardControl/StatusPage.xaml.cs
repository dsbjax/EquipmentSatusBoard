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
    /// Interaction logic for StatusPage.xaml
    /// </summary>
    public partial class StatusPage : UserControl
    {
        internal bool HasBackgroundImage {  get { return backgroundImage.Source != null; } }
        private string backgroundImageFilename = "";

        public StatusPage()
        {
            InitializeComponent();
        }

        public StatusPage(StreamReader statusLines)
        {
            InitializeComponent();

            string line = statusLines.ReadLine();
            while (!statusLines.EndOfStream && !line.StartsWith("Page End"))
            {
                if (line.StartsWith("Background Image: "))
                {
                    backgroundImageFilename = line.Replace("Background Image: ", null);

                    if (File.Exists(backgroundImageFilename))
                        backgroundImage.Source = new BitmapImage(new Uri(backgroundImageFilename));

                    line = statusLines.ReadLine();
                }

                while (line.StartsWith("Start Group:"))
                {
                    var group = new EquipmentGroup(line.Remove(0, 13), statusLines);
                    group.EquipmentGroupDelete += GroupDelete;
                    DockPanel.SetDock(group, Dock.Top);

                    page.Children.Add(group);

                    line = statusLines.ReadLine();
                }
            }
        }

        private void GroupDelete(object sender, EquipmentGroupDeleteEventArgs e)
        {
            page.Children.Remove(e.EquipmentGroup);
        }

        internal void SetBackgroundImage(string filename)
        {
            if (filename == null)
            {
                backgroundImage.Source = null;
                return;
            }

            backgroundImageFilename = filename;

            if (File.Exists(backgroundImageFilename))
                backgroundImage.Source = new BitmapImage(new Uri(backgroundImageFilename));
        }

        public override string ToString()
        {
            string output = "";

            output += "Page Start\n";
            output += "Background Image: " + backgroundImageFilename + "\n";

            foreach (var item in page.Children)
                output += item.ToString();

            output += "Page End\n";

            return output;
        }
    }
}
