using EdgeDetection.Implementations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        private static IConverter converter = new CSharpImplementation();

        public static void runConversion()
        {
            if (inputPath == null || inputPath.Equals("") || outputPath == null || outputPath.Equals(""))
            {
                MessageBox.Show("Select paths for input and output");
                return;
            }
            converter.Convert(inputPath, outputPath, cores, threshold);
        }

        public static void runMeasurements()
        {
            MessageBox.Show("Performing measurements");
        }
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
        }
        public static void SetOutputPath(string incomingPath)
        {
            outputPath = incomingPath;
        }
        public static void SetImplementation(IConverter incomingConverter)
        {
            converter = incomingConverter;
        }

    }
}
