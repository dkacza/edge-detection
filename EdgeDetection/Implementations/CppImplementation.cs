using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdgeDetection.Implementations
{
    internal class CppImplementation : IConverter
    {
        public void Convert(string inputPath, string outputPath, int cores, int threshold)
        {
            String diagnosticMsg = $"Converting with C++.\nInput path: {inputPath}\nOutput path: {outputPath}\nThreshold: {threshold}\nCores: {cores}";
            MessageBox.Show(diagnosticMsg);
        }
    }
}
