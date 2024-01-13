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
        private static IConverter converter = new CSharpImplementation(); // Selected implementation
        private static ImageOverview imageOverviewInstance; // Instance of image view to display previews
        private static Actions actionsViewInstance;

        // Run single conversion with specified threshold and core number.
        public static void runConversion()
        {
            // Check if paths are valid
            if (inputPath == null || inputPath.Equals("") || outputPath == null || outputPath.Equals(""))
            {
                MessageBox.Show("Select paths for input and output");
                return;
            }

            // Get current timestamp for unique file name
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            string outputFileName = "conv_" + now.ToUnixTimeMilliseconds() + ".bmp";

            // Display CPU ticks
            long timeResult = converter.Convert(inputPath, outputPath + "\\" + outputFileName, cores);
            actionsViewInstance.cpuTicks.Text = "CPU ticks: " + timeResult;

            // Display preview
            imageOverviewInstance.DisplayOutputImage(outputPath + "\\" + outputFileName);
        }

        // Run conversion for both x86 assembly and C# DLL. Run it for every thread configuration possible.
        public static void runMeasurements()
        {
            MessageBox.Show("Press OK to start performing the measurements. This might take a while");
            if (inputPath == null || inputPath.Equals("") || outputPath == null || outputPath.Equals(""))
            {
                MessageBox.Show("Select paths for input and output");
                return;
            }

            List<int> threadConfigs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 16, 20, 24 };
            IConverter csConverter = new CSharpImplementation();
            IConverter asmConverter = new AsmImplementation();
            string csvOutput = "threads,cs,asm\n";

            for (int i = 0; i < threadConfigs.Count; i++)
            {
                int threads = threadConfigs[i];
                long csScore = csConverter.Measure(inputPath, threads);
                long asmScore = asmConverter.Measure(inputPath, threads);
                string csvLine = threads.ToString() + "," + csScore.ToString() + "," + asmScore.ToString() + "\n";
                csvOutput += csvLine;
            }
            File.WriteAllText("results.csv", csvOutput);
            MessageBox.Show("Results saved to results.csv");
        }


        // Getters and setters
        public static void SetCores(int incomingCores)
        {
            cores = incomingCores;
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
