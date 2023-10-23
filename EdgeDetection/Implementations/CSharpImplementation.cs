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
            String diagnosticMsg = $"Converting with C#.\nInput path: {inputPath}\nOutput path: {outputPath}\nThreshold: {threshold}\nCores: {cores}";
            MessageBox.Show(diagnosticMsg);

            Bitmap inputImage = new Bitmap(inputPath);


            /*BITMAP TO BYTE ARRAY TEST*/

            int[] imageData = BitmapConverter.convertBitmapToIntArray(inputImage);
            Bitmap bitmapFromIntArr = BitmapConverter.ConvertIntArrayToBitmap(imageData, inputImage.Width, inputImage.Height);
            Bitmap outputImage = CsEdgeDetection.ApplySobelEdgeDetection(bitmapFromIntArr, threshold);



            /*END OF TEST*/

            // Bitmap outputImage = CsEdgeDetection.ApplySobelEdgeDetection(inputImage, threshold);

            outputImage.Save(outputPath + "\\converted.jpg", ImageFormat.Jpeg);
        }
    }
}
