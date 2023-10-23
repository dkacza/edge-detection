using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EdgeDetection.Implementations
{
    internal class BitmapConverter
    {
        public static int[] convertBitmapToIntArray(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[] pixels = new int[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);
                    int pixelValue = 0;
                    pixelValue = pixelColor.R << 16 | pixelColor.G << 8 | pixelColor.B;
                    pixels[y * width + x] = pixelValue;
                }
            }

            return pixels;
        }

        public static Bitmap ConvertIntArrayToBitmap(int[] pixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte r = ((byte)(pixels[y  * width + x] >> 16));
                    byte g = ((byte)(pixels[y  * width + x] >> 8));
                    byte b = (byte)(pixels[y  * width + x]);
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(r, g, b);
                    bitmap.SetPixel(x, y, color);
                }
            }
            return bitmap;
        }
    }
}
