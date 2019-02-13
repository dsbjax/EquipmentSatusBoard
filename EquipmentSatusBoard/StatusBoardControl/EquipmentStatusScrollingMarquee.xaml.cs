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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EquipmentSatusBoard.StatusBoardControl
{
    /// <summary>
    /// Interaction logic for EquipmentStatusScrollingMarquee.xaml
    /// </summary>
    public partial class EquipmentStatusScrollingMarquee : UserControl
    {
        private static TextBlock equipmentStatus = new TextBlock()
        {
            FontFamily = new FontFamily("Times New Roman"),
            FontSize = 36,
            Foreground = Brushes.White,
            FontWeight = FontWeights.Bold,
            Background = null
        };

        private static Canvas marqueeCanvas = new Canvas()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            Height = 40,
            ClipToBounds = true,
            Margin = new Thickness(3)
        };

        public EquipmentStatusScrollingMarquee()
        {
            InitializeComponent();
            marqueeCanvas.Children.Add(equipmentStatus);
            groupBox.Children.Add(marqueeCanvas);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = -2 * equipmentStatus.ActualWidth;
            doubleAnimation.To = marqueeCanvas.ActualWidth;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:30"));
            equipmentStatus.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

        public static void AddEquipmentStatusText(string text)
        {
            if (equipmentStatus.Text.Length > 0)
                equipmentStatus.Text += "\t";

            equipmentStatus.Text += text;

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = -2 * equipmentStatus.ActualWidth;
            doubleAnimation.To = marqueeCanvas.ActualWidth;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:30"));
            equipmentStatus.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

        public static void RemoveEquipmentStatusText(string text)
        {
            equipmentStatus.Text.Replace(text, "");
            equipmentStatus.Text.Replace("\t\t", "\t");

            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = -2 * equipmentStatus.ActualWidth;
            doubleAnimation.To = marqueeCanvas.ActualWidth;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:30"));
            equipmentStatus.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

    }
}
