using EdgeDetection.Implementations;
using EdgeDetection.View.Actions;
using EdgeDetection.View.ImageOverview;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EdgeDetection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string? inputPath;
        private static string? outputPath;
        private static int cores = 1;
        private static int threshold = 0; 
        private static IConverter converter = new CSharpImplementation(); // Selected implementation
        private static ImageOverview imageOverviewInstance; // Instance of image view to display previews
        private static Actions actionsViewInstance;

        // Run single conversion with specified threshold and core number.
        public static void runConversion()
        {
            actionsViewInstance.cpuTicks.Text = "";
            if (inputPath == null || inputPath.Equals("") || outputPath == null || outputPath.Equals(""))
            {
                MessageBox.Show("Select paths for input and output");
                return;
            }

            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            string outputFileName = "conv_" + now.ToUnixTimeMilliseconds() + ".bmp";


            long timeResult = converter.Convert(inputPath, outputPath + "\\" + outputFileName, cores, threshold);
            actionsViewInstance.cpuTicks.Text = "CPU ticks: " + timeResult;

            imageOverviewInstance.DisplayOutputImage(outputPath + "\\" + outputFileName);
        }
        // Run conversion for both x86 assembly and C# DLL. Run it for every thread configuration possible.
        public static void runMeasurements()
        {
            MessageBox.Show("Performing measurements");
        }


        // Getters and setters
        public static void SetCores(int incomingCores)
        {
            cores = incomingCores;
        }
        public static void SetThreshold(int incomingThreshold)
        {
            threshold = incomingThreshold;
        }

        public static void SetInputPath(string incomingPath)
        {
            inputPath = incomingPath;
            imageOverviewInstance.DisplayInputImage(inputPath);
        }
        public static void SetOutputPath(string incomingPath)
        {
            outputPath = incomingPath;
        }
        public static void SetImplementation(IConverter incomingConverter)
        {
            converter = incomingConverter;
        }
        public static void SetImageOverview(ImageOverview incomingImageOverview)
        {
            imageOverviewInstance = incomingImageOverview;
        }
        public static void SetActionView(Actions actions)
        {
            actionsViewInstance = actions;
        }

    }
}
