using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLibrary
{
    public static class CsEdgeDetection
    {
        public static unsafe void convert(byte[] input, int startIndex, byte[] output, int pixelsToProcess)
        {
            int index = startIndex;
            for (int i = 0; i < pixelsToProcess; i++)
            {
                byte grayscale = getGrayscale(input, index);

                output[index - startIndex] = grayscale;
                output[index - startIndex + 1] = grayscale;
                output[index - startIndex + 2] = grayscale;

                index += 3;
            }
        }
        private static byte getGrayscale(byte[] input, int index)
        {
            byte blue = input[index];
            byte green = input[index + 1];
            byte red = input[index + 2];

            byte grayscale = (byte)(0.299 * red + 0.587 * green + 0.114 * blue);

            return grayscale;
        }
    }
}
