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

namespace WritingFloatsToBitmap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WriteableBitmap b = new WriteableBitmap(5, 5, 96, 96, PixelFormats.Prgba128Float, null);
            byte[] pixels = new byte[b.PixelWidth * b.PixelHeight * 16]; // 16 bytes per pixel
            double[] values = new double[b.PixelWidth * b.PixelHeight];

            Random random = new Random();
            List<datapoint> points = new List<datapoint>();
            for (int i = 0; i < 10; i++)
            {
                var point = new datapoint();
                point.X = random.NextDouble() * (b.PixelWidth - 1);
                point.Y = random.NextDouble() * (b.PixelHeight - 1);
                points.Add(point);
            }

            for (int i = 0; i < points.Count; i++)
            {
                var posX = points[i].X;
                var posY = points[i].Y;

                var pos = (int)(b.PixelWidth * posX + posY);
                values[pos] = 1;

                Array.Copy(BitConverter.GetBytes((float)values[pos]), 0, pixels, pos * 16, 4);
                Array.Copy(BitConverter.GetBytes((float)values[pos]), 0, pixels, pos * 16 + 4, 4);
                Array.Copy(BitConverter.GetBytes((float)values[pos]), 0, pixels, pos * 16 + 8, 4);
                Array.Copy(BitConverter.GetBytes((float)values[pos]), 0, pixels, pos * 16 + 12, 4);
            }

            b.WritePixels(new Int32Rect(0, 0, b.PixelWidth, b.PixelHeight), pixels, b.PixelWidth * 16, 0);
            img.Source = b;
        }
    }

    public class datapoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
