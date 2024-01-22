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
                int rowIndex = (y * width);
                int outputRowIndex = ((y - startY) * width);


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
                    int cellIndex = rowIndex + (x);
                    int outputCellIndex = outputRowIndex + (x);

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
                    int middleCellFromUpperRowIndex = cellIndex - (width);
                    // Top left
                    verticalKernelSum += (-1) * input[middleCellFromUpperRowIndex - 1];
                    horizontalKernelSum += (1) * input[middleCellFromUpperRowIndex - 1];
                    // Top center
                    verticalKernelSum += 0;
                    horizontalKernelSum += (2) * input[middleCellFromUpperRowIndex];
                    // Top right
                    verticalKernelSum += (1) * input[middleCellFromUpperRowIndex + 1];
                    horizontalKernelSum += (1) * input[middleCellFromUpperRowIndex + 1];

                    // Center (curent) row
                    // Center left
                    verticalKernelSum += (-2) * input[cellIndex - 1];
                    horizontalKernelSum += 0;
                    // Center center
                    verticalKernelSum += 0;
                    horizontalKernelSum += 0;
                    // Center Right
                    verticalKernelSum += (2) * input[cellIndex + 1];
                    horizontalKernelSum += 0;

                    // Bottom row
                    // Bottom row is accessed by adding the size of one row to the current cell index
                    int middleCellFromBottomRowIndex = cellIndex + (width);
                    // Bottom left
                    verticalKernelSum += (-1) * input[middleCellFromBottomRowIndex - 1];
                    horizontalKernelSum += (-1) * input[middleCellFromBottomRowIndex - 1];
                    // Bottom center
                    verticalKernelSum += 0;
                    horizontalKernelSum += (-2) * input[middleCellFromBottomRowIndex];
                    // Bottom right
                    verticalKernelSum += (1) * input[middleCellFromBottomRowIndex + 1];
                    horizontalKernelSum += (-1) * input[middleCellFromBottomRowIndex + 1];


                    // Edge precence value is calculated as geometric mean of both horizontal and vertical values. It's clamped between 0-255 to fit within byte size.
                    int edgePresenceValue = (int)Math.Sqrt(horizontalKernelSum * horizontalKernelSum + verticalKernelSum * verticalKernelSum);
                    edgePresenceValue = edgePresenceValue > 255 ? 255 : edgePresenceValue < 0 ? 0 : edgePresenceValue;


                    // The value is saved to the output table
                    output[outputCellIndex] = (byte)edgePresenceValue;
                    x++;
                }
                outputRow++;
                y++;
            }

        }
    }
}
