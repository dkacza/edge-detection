using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeDetection.Implementations
{
    internal class GrayscaleService
    {
        public static byte[] ConvertToGrayscale(byte[] inputWithoutHeader, byte[] grayscaleData, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                int rowIndex = (i * width * 3);
                int outputRowIndex = (i * width);
                for (int j = 0; j < width; j++)
                {
                    int cellIndex = rowIndex + (j * 3);
                    int outputCellIndex = outputRowIndex + j;

                    byte blue = inputWithoutHeader[cellIndex];
                    byte green = inputWithoutHeader[cellIndex + 1];
                    byte red = inputWithoutHeader[cellIndex + 2];

                    byte grayscale = (byte)(0.299 * red + 0.587 * green + 0.114 * blue);
                    grayscaleData[outputCellIndex] = grayscale;
                }
            }

            return grayscaleData;
        }
    }
}
