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

namespace EdgeDetection.View.ImageOverview
{
    /// <summary>
    /// Interaction logic for ImageOverview.xaml
    /// </summary>
    public partial class ImageOverview : UserControl
    {
        public ImageOverview()
        {
            InitializeComponent();
        }

        public void DisplayInputImage(string? inputPath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(inputPath);
            image.EndInit();      

            inputImageViewer.Source = image;
        }
        public void DisplayOutputImage(string? outputPath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(outputPath);
            image.EndInit();

            outputImageViewer.Source = image;
        }

    }
}
