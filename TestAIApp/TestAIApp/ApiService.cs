using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

public class ApiService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<JObject> GetPredictions(string imagePath)
    {
        var json = new JObject { { "image_path", imagePath } };
        var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("http://localhost:5000/predict", content);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return JObject.Parse(responseBody);
    }
}

public class ImageProcessor
{
    public static Bitmap DrawBoundingBoxes(string imagePath, JArray boundingBoxes, JArray labels, JArray confidences)
    {
        Bitmap originalImage = new Bitmap(imagePath);
        Bitmap image = new Bitmap(originalImage.Width, originalImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        using (Graphics g = Graphics.FromImage(image))
        {
            g.DrawImage(originalImage, 0, 0);
        }

        using (Graphics graphics = Graphics.FromImage(image))
        {
            Pen pen1 = new Pen(Color.Red, 2);
            Pen pen2 = new Pen(Color.Blue, 2);
            Font font = new Font("Arial", 12);
            Brush brush = new SolidBrush(Color.Yellow);

            for (int i = 0; i < boundingBoxes.Count; i++)
            {
                var box = boundingBoxes[i];
                var label = labels[i].ToString();
                var confidence = confidences[i].ToObject<double>();

                var x1 = (int)box[0];
                var y1 = (int)box[1];
                var x2 = (int)box[2];
                var y2 = (int)box[3];
                
                if(label == "1")
                    graphics.DrawRectangle(pen1, x1, y1, x2 - x1, y2 - y1);
                else
                    graphics.DrawRectangle(pen2, x1, y1, x2 - x1, y2 - y1);

                string labelText = $"{label}: {confidence:F2}";
                graphics.DrawString(labelText, font, brush, new PointF(x1, y1 - 20));
            }
        }

        return image;
    }

    public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
    {
        using (var memory = new System.IO.MemoryStream())
        {
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}