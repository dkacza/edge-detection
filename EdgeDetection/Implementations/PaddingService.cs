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
            byte[] paddedByteArray = new byte[newWidth * newHeight * 3];

            // Copy the original image data to the center of the padded image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the source and destination indices
                    int srcIndex = (y * width + x) * 3;
                    int destIndex = ((y + 1) * newWidth + x + 1) * 3;

                    // Copy RGB values to the padded image
                    paddedByteArray[destIndex] = byteArray[srcIndex];
                    paddedByteArray[destIndex + 1] = byteArray[srcIndex + 1];
                    paddedByteArray[destIndex + 2] = byteArray[srcIndex + 2];
                }
            }

            // Copy the bordering pixels to the new border
            for (int x = 0; x < width; x++)
            {
                int topSrcIndex = x * 3;
                int bottomSrcIndex = ((height - 1) * width + x) * 3;

                // Top border
                int topDestIndex = x * 3;
                paddedByteArray[topDestIndex] = byteArray[topSrcIndex];
                paddedByteArray[topDestIndex + 1] = byteArray[topSrcIndex + 1];
                paddedByteArray[topDestIndex + 2] = byteArray[topSrcIndex + 2];

                // Bottom border
                int bottomDestIndex = ((newHeight - 1) * newWidth + x) * 3;
                paddedByteArray[bottomDestIndex] = byteArray[bottomSrcIndex];
                paddedByteArray[bottomDestIndex + 1] = byteArray[bottomSrcIndex + 1];
                paddedByteArray[bottomDestIndex + 2] = byteArray[bottomSrcIndex + 2];
            }

            for (int y = 0; y < height; y++)
            {
                int leftSrcIndex = (y * width) * 3;
                int rightSrcIndex = (y * width + (width - 1)) * 3;

                // Left border
                int leftDestIndex = ((y + 1) * newWidth) * 3;
                paddedByteArray[leftDestIndex] = byteArray[leftSrcIndex];
                paddedByteArray[leftDestIndex + 1] = byteArray[leftSrcIndex + 1];
                paddedByteArray[leftDestIndex + 2] = byteArray[leftSrcIndex + 2];

                // Right border
                int rightDestIndex = ((y + 1) * newWidth + newWidth - 1) * 3;
                paddedByteArray[rightDestIndex] = byteArray[rightSrcIndex];
                paddedByteArray[rightDestIndex + 1] = byteArray[rightSrcIndex + 1];
                paddedByteArray[rightDestIndex + 2] = byteArray[rightSrcIndex + 2];
            }

            // Fill the corner pixels (top-left, top-right, bottom-left, bottom-right) using the closest border pixels
            paddedByteArray[0] = byteArray[0]; // Top-left corner
            paddedByteArray[3] = byteArray[(width - 1) * 3]; // Top-right corner
            paddedByteArray[(newWidth - 1) * 3] = byteArray[(height - 1) * width * 3]; // Bottom-left corner
            paddedByteArray[(newWidth - 1) * 3 + 3] = byteArray[(height - 1) * width * 3 + (width - 1) * 3]; // Bottom-right corner

            return paddedByteArray;
        }

        public byte[] removePadding(byte[] paddedPixels, int paddedWidth, int paddedHeight)
        {
            int width = paddedWidth - 2;
            int height = paddedHeight - 2;

            byte[] originalPixels = new byte[width * height * 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = ((y + 1) * paddedWidth + x + 1) * 3;
                    int dstIndex = (y * width + x) * 3;

                    originalPixels[dstIndex] = paddedPixels[srcIndex];
                    originalPixels[dstIndex + 1] = paddedPixels[srcIndex + 1];
                    originalPixels[dstIndex + 2] = paddedPixels[srcIndex + 2];
                }
            }
            return originalPixels;
        }
    }
}
