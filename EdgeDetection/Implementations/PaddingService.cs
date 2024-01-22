using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeDetection.Implementations
{
    internal class PaddingService
    {
        public byte[] padBitmap(byte[] byteArray, int width, int height)
        {
            // Define the new dimensions for the padded image
            int newWidth = width + 2;
            int newHeight = height + 2;

            // Create a new byte array to hold the padded image data
            byte[] paddedByteArray = new byte[newWidth * newHeight];

            // Copy the original image data to the center of the padded image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the source and destination indices
                    int srcIndex = (y * width + x);
                    int destIndex = ((y + 1) * newWidth + x + 1);

                    // Copy grayscale value to the padded image
                    paddedByteArray[destIndex] = byteArray[srcIndex];
                }
            }

            // Copy the bordering pixels to the new border
            for (int x = 0; x < width; x++)
            {
                int topSrcIndex = x;
                int bottomSrcIndex = ((height - 1) * width + x);

                // Top border
                int topDestIndex = x;
                paddedByteArray[topDestIndex] = byteArray[topSrcIndex];

                // Bottom border
                int bottomDestIndex = ((newHeight - 1) * newWidth + x);
                paddedByteArray[bottomDestIndex] = byteArray[bottomSrcIndex];
            }

            for (int y = 0; y < height; y++)
            {
                int leftSrcIndex = (y * width);
                int rightSrcIndex = (y * width + (width - 1));

                // Left border
                int leftDestIndex = ((y + 1) * newWidth);
                paddedByteArray[leftDestIndex] = byteArray[leftSrcIndex];

                // Right border
                int rightDestIndex = ((y + 1) * newWidth + newWidth - 1);
                paddedByteArray[rightDestIndex] = byteArray[rightSrcIndex];
            }

            // Fill the corner pixels (top-left, top-right, bottom-left, bottom-right) using the closest border pixels
            paddedByteArray[0] = byteArray[0]; // Top-left corner
            paddedByteArray[3] = byteArray[(width - 1)]; // Top-right corner
            paddedByteArray[(newWidth - 1)] = byteArray[(height - 1) * width]; // Bottom-left corner
            paddedByteArray[(newWidth - 1) + 3] = byteArray[(height - 1) * width + (width - 1)]; // Bottom-right corner

            return paddedByteArray;
        }

        public byte[] removePadding(byte[] paddedPixels, int paddedWidth, int paddedHeight)
        {
            int width = paddedWidth - 2;
            int height = paddedHeight - 2;

            byte[] originalPixels = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = ((y + 1) * paddedWidth + x + 1);
                    int dstIndex = (y * width + x);

                    originalPixels[dstIndex] = paddedPixels[srcIndex];
                }
            }
            return originalPixels;
        }
    }
}
