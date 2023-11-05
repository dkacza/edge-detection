using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdgeDetection.Implementations
{
    internal class AsmImplementation : IConverter
    {
        public long Convert(string inputPath, string outputPath, int cores, int threshold)
        {
            String diagnosticMsg = $"Converting with x86 assembly.\nInput path: {inputPath}\nOutput path: {outputPath}\nThreshold: {threshold}\nCores: {cores}";
            MessageBox.Show(diagnosticMsg);
            return 0L;
        }
    }
}
