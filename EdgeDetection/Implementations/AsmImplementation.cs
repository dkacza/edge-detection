using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdgeDetection.Implementations
{
    internal class AsmImplementation : IConverter
    {
        [DllImport(@"D:\Dev\CS\edge-detection\x64\Debug\AsmLibrary.dll")]
        static extern int MyProc1(int a, int b);
        public long Convert(string inputPath, string outputPath, int cores, int threshold)
        {

            int x = 5, y = 3;
            int retVal = MyProc1(x, y);
            string message = "x86 assembly result:" + retVal;


            MessageBox.Show(message);
            return 0L;
        }
    }
}
