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
            Bitmap outputImage = CsEdgeDetection.ApplySobelEdgeDetection(inputImage, threshold);

            outputImage.Save(outputPath + "\\converted.jpg", ImageFormat.Jpeg);
        }
    }
}
