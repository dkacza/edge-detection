using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsLibrary;

namespace EdgeDetection.Implementations
{
    public class CSharpImplementation : IConverter
    {
        public void Convert(string inputPath, string outputPath, int cores, int threshold)
        {

            int testVal = CsImplementation.Test(1, 2);
            String diagnosticMsg = $"Converting with C#.\nInput path: {inputPath}\nOutput path: {outputPath}\nThreshold: {threshold}\nCores: {cores}\nTest value from DLL: {testVal}";
            MessageBox.Show(diagnosticMsg);

            Bitmap inputImage = new Bitmap(inputPath); // Load your input image
            Bitmap outputImage = ApplySobelEdgeDetection(inputImage, threshold);

            outputImage.Save(outputPath + "\\converted.jpg", ImageFormat.Jpeg);


        }
        public static Bitmap ApplySobelEdgeDetection(Bitmap inputImage, int threshold)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;

            Bitmap outputImage = new Bitmap(width, height);

            int[,] sobelX = {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            int[,] sobelY = {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int gx = 0;
                    int gy = 0;

                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            Color pixel = inputImage.GetPixel(x + i, y + j);
                            int grayValue = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);

                            gx += grayValue * sobelX[j + 1, i + 1];
                            gy += grayValue * sobelY[j + 1, i + 1];
                        }
                    }

                    int gradient = (int)Math.Sqrt(gx * gx + gy * gy);
                    gradient = Math.Min(255, Math.Max(0, gradient)); // Clamp to 0-255

                    // Apply the threshold for edge detection
                    if (gradient >= threshold)
                    {
                        Color newPixel = Color.FromArgb(255, 255, 255); // White for edges
                        outputImage.SetPixel(x, y, newPixel);
                    }
                    else
                    {
                        Color newPixel = Color.FromArgb(0, 0, 0); // Black for non-edges
                        outputImage.SetPixel(x, y, newPixel);
                    }
                }
            }

            return outputImage;
        }

    }
}
