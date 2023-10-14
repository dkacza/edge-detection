using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeDetection
{
    public interface IConverter
    {
        public void Convert(string inputPath, string outputPath, int cores, int threshold); 
    }
}
