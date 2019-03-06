using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace EquipmentSatusBoard.StatusBoardControl
{
    /// <summary>
    /// Interaction logic for EquipmentStatusScrollingMarquee.xaml
    /// </summary>
    public partial class EquipmentStatusScrollingMarquee : UserControl
    {
        private const string DELIMITOR = "  *  ";

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

            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = -2 * equipmentStatus.ActualWidth,
                To = marqueeCanvas.ActualWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.Parse("0:0:30"))
            };
            equipmentStatus.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

        public static void AddEquipmentStatusText(string text)
        {
            if (equipmentStatus.Text.Length > 0)
                equipmentStatus.Text += DELIMITOR;

            equipmentStatus.Text += text;

            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = -2 * equipmentStatus.ActualWidth,
                To = marqueeCanvas.ActualWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.Parse("0:0:30"))
            };
            equipmentStatus.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

        public static void RemoveEquipmentStatusText(string text)
        {
            equipmentStatus.Text = equipmentStatus.Text.Replace(text, null);
            equipmentStatus.Text = equipmentStatus.Text.Replace(DELIMITOR + DELIMITOR, DELIMITOR);
            if (equipmentStatus.Text.EndsWith(DELIMITOR))
                equipmentStatus.Text = equipmentStatus.Text.Remove(equipmentStatus.Text.Length - DELIMITOR.Length);

            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = -equipmentStatus.ActualWidth,
                To = marqueeCanvas.ActualWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.Parse("0:0:30"))
            };
            equipmentStatus.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

    }
}
