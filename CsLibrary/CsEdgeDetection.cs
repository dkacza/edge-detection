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
        public static unsafe void convert(byte[] input, byte[] output, int width, int height, int startY, int endY)
        {
            int BYTES_PER_PIXEL = 3;
            int y = startY;
            int outputRow = 0;
            // Process each row
            while (y <= endY)
            {
                // If on top or bottom edge, proceed to the next row
                if (y == 0 || y == height - 1)
                {
                    y++;
                    outputRow++;
                    continue;
                }

                // Calculate the indexes of first cell in input and output array
                int rowIndex = (y * width * BYTES_PER_PIXEL);
                int outputRowIndex = ((y - startY) * width * BYTES_PER_PIXEL);


                // Process each cell in a row
                int x = 0;
                while (x < width)
                {
                    // If on left or right edge, proceed to the next cell
                    if (x == 0 || x == width - 1)
                    {
                        x++;
                        continue;
                    }

                    // Calculate current cell indexes for input and output array
                    int cellIndex = rowIndex + (x * BYTES_PER_PIXEL);
                    int outputCellIndex = outputRowIndex + (x * BYTES_PER_PIXEL);

                    // Declaring variables for the both vertical and horizontal kernels
                    int verticalKernelSum = 0;
                    // Vertical kernel values
                    // -1 0 1
                    // -2 0 2
                    // -1 0 1

                    int horizontalKernelSum = 0;
                    // Horizontal kerrnel values
                    //  1  2  1
                    //  0  0  0
                    // -1 -2 -1


                    // Upper row
                    // Upper row is accessed by subtracting the size of one row from the current cell index
                    int middleCellFromUpperRowIndex = cellIndex - (width * BYTES_PER_PIXEL);
                    // Top left
                    verticalKernelSum += (-1) * getGrayscale(input, middleCellFromUpperRowIndex - BYTES_PER_PIXEL);
                    horizontalKernelSum += (1) * getGrayscale(input, middleCellFromUpperRowIndex - BYTES_PER_PIXEL);
                    // Top center
                    verticalKernelSum += 0;
                    horizontalKernelSum += (2) * getGrayscale(input, middleCellFromUpperRowIndex);
                    // Top right
                    verticalKernelSum += (1) * getGrayscale(input, middleCellFromUpperRowIndex + BYTES_PER_PIXEL);
                    horizontalKernelSum += (1) * getGrayscale(input, middleCellFromUpperRowIndex + BYTES_PER_PIXEL);

                    // Center (curent) row
                    // Center left
                    verticalKernelSum += (-2) * getGrayscale(input, cellIndex - BYTES_PER_PIXEL);
                    horizontalKernelSum += 0;
                    // Center center
                    verticalKernelSum += 0;
                    horizontalKernelSum += 0;
                    // Center Right
                    verticalKernelSum += (2) * getGrayscale(input, cellIndex + BYTES_PER_PIXEL);
                    horizontalKernelSum += 0;

                    // Bottom row
                    // Bottom row is accessed by adding the size of one row to the current cell index
                    int middleCellFromBottomRowIndex = cellIndex + (width * BYTES_PER_PIXEL);
                    // Bottom left
                    verticalKernelSum += (-1) * getGrayscale(input, middleCellFromBottomRowIndex - BYTES_PER_PIXEL);
                    horizontalKernelSum += (-1) * getGrayscale(input, middleCellFromBottomRowIndex - BYTES_PER_PIXEL);
                    // Bottom center
                    verticalKernelSum += 0;
                    horizontalKernelSum += (-2) * getGrayscale(input, middleCellFromBottomRowIndex);
                    // Bottom right
                    verticalKernelSum += (1) * getGrayscale(input, middleCellFromBottomRowIndex + BYTES_PER_PIXEL);
                    horizontalKernelSum += (-1) * getGrayscale(input, middleCellFromBottomRowIndex + BYTES_PER_PIXEL);


                    // Edge precence value is calculated as geometric mean of both horizontal and vertical values. It's clamped between 0-255 to fit within byte size.
                    int edgePresenceValue = (int)Math.Sqrt(horizontalKernelSum * horizontalKernelSum + verticalKernelSum * verticalKernelSum);
                    edgePresenceValue = edgePresenceValue > 255 ? 255 : edgePresenceValue < 0 ? 0 : edgePresenceValue;


                    // The value is saved to the output table
                    output[outputCellIndex] = (byte)edgePresenceValue;
                    output[outputCellIndex + 1] = (byte)edgePresenceValue;
                    output[outputCellIndex + 2] = (byte)edgePresenceValue;

                    x++;
                }
                outputRow++;
                y++;
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
