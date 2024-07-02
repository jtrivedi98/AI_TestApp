using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;


namespace TestAIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ProcessImage("C:\\Users\\Jeet Trivedi\\Desktop\\Mozart IQ Failed Blender Case\\1234\\1234\\1234\\0\\1234_1234_0_0.DCM");
        }

        private async void ProcessImage(string imagePath)
        {
            var apiService = new ApiService();
            var response = await apiService.GetPredictions(imagePath);

            var boundingBoxes = response["bounding_boxes"] as JArray;
            var labels = response["labels"] as JArray;
            var confidences = response["confidences"] as JArray;

            Bitmap imageWithBoxes = ImageProcessor.DrawBoundingBoxes(imagePath, boundingBoxes, labels, confidences);
            BitmapImage bitmapImage = ImageProcessor.ConvertBitmapToBitmapImage(imageWithBoxes);

            ProcessedImage.Source = bitmapImage;
        }
    }
}
