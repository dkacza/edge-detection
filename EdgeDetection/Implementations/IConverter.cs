using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeDetection.Implementations
{
    public interface IConverter
    {
        public long Convert(string inputPath, string outputPath, int cores);
        public long Measure(string inputPath, int cores);
        
    }
}
