using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdgeDetection.Implementations
{
    internal class AsmImplementation : IConverter
    {
        [DllImport(@"D:\Dev\CS\edge-detection\x64\Debug\AsmLibrary.dll")]
        static extern void APPLY_FLITER(byte[] input, byte[] output, int width, int height, int startY, int endY);
        public long Convert(string inputPath, string outputPath, int cores)
        {
            // Instatiate services
            FileService fileService = new FileService();
            PaddingService paddingService = new PaddingService();

            // Load image
            // If the bitmap is empty, show message and return immeadiately
            byte[] bitmapFromFile = fileService.LoadImageAsBytes(inputPath);
            if (bitmapFromFile.Length == 0)
            {
                MessageBox.Show("Error occured while reading bitmap file.");
                return 0;
            }

            // Read Metadata
            int dataOffset = BitConverter.ToInt32(bitmapFromFile, 10);
            int width = BitConverter.ToInt32(bitmapFromFile, 18);
            int height = BitConverter.ToInt32(bitmapFromFile, 22);
            int bitsPerPixel = BitConverter.ToInt16(bitmapFromFile, 28);
            int bytesPerPixel = bitsPerPixel / 8;


            // Detach header
            byte[] bitmapHeader = new byte[dataOffset];
            byte[] bitmapWithoutHeader = new byte[bitmapFromFile.Length - dataOffset];
            Array.Copy(bitmapFromFile, bitmapHeader, dataOffset);
            Array.Copy(bitmapFromFile, dataOffset, bitmapWithoutHeader, 0, bitmapWithoutHeader.Length);


            // Pixel padding
            byte[] paddedBitmap = paddingService.padBitmap(bitmapWithoutHeader, width, height);
            int paddedWidth = width + 2;
            int paddedHeight = height + 2;

            // Split data for threads
            List<ConversionTask> conversionTasks = ConversionTask.splitForTasks(paddedWidth, paddedHeight, cores, bytesPerPixel);
            Thread[] threadArray = new Thread[cores];
            for (int i = 0; i < cores; i++)
            {
                int startY = conversionTasks[i].startY;
                int endY = conversionTasks[i].endY;
                byte[] output = conversionTasks[i].outputData;

                Thread thread = new Thread(() =>
                {
                    APPLY_FLITER(paddedBitmap, output, paddedWidth, paddedHeight, startY, endY);

                });
                threadArray[i] = thread;

            }

            // Run tasks and perform time measurement
            long timeStart = Stopwatch.GetTimestamp();
            for (int i = 0; i < cores; i++)
            {
                threadArray[i].Start();
            }
            for (int i = 0; i < cores; i++)
            {
                threadArray[i].Join();
            }
            long timeEnd = Stopwatch.GetTimestamp();


            // Concatenate processed chunks of bitmap
            byte[] concatenatedChunks = Array.Empty<byte>();
            conversionTasks.OrderBy(task => task.orderIndex).ToList().ForEach(task =>
            {
                concatenatedChunks = concatenatedChunks.Concat(task.outputData).ToArray();
            });

            // Remove padding and attach header
            byte[] concatenatedChunksWithoutPadding = paddingService.removePadding(concatenatedChunks, paddedWidth, paddedHeight);
            byte[] outputBitmap = new byte[bitmapFromFile.Length];
            bitmapHeader.CopyTo(outputBitmap, 0);
            Array.Copy(concatenatedChunksWithoutPadding, 0, outputBitmap, dataOffset, concatenatedChunksWithoutPadding.Length);


            // Save to file
            fileService.SaveByteArray(outputPath, outputBitmap);

            return timeEnd - timeStart;
        }

        public long Measure(string inputPath, int cores)
        {
            FileService fileService = new FileService();
            PaddingService paddingService = new PaddingService();

            // Load image
            // If the bitmap is empty, show message and return immeadiately
            byte[] bitmapFromFile = fileService.LoadImageAsBytes(inputPath);
            if (bitmapFromFile.Length == 0)
            {
                MessageBox.Show("Error occured while reading bitmap file.");
                return 0;
            }

            // Read Metadata
            int dataOffset = BitConverter.ToInt32(bitmapFromFile, 10);
            int width = BitConverter.ToInt32(bitmapFromFile, 18);
            int height = BitConverter.ToInt32(bitmapFromFile, 22);
            int bitsPerPixel = BitConverter.ToInt16(bitmapFromFile, 28);
            int bytesPerPixel = bitsPerPixel / 8;


            // Detach header
            byte[] bitmapHeader = new byte[dataOffset];
            byte[] bitmapWithoutHeader = new byte[bitmapFromFile.Length - dataOffset];
            Array.Copy(bitmapFromFile, bitmapHeader, dataOffset);
            Array.Copy(bitmapFromFile, dataOffset, bitmapWithoutHeader, 0, bitmapWithoutHeader.Length);


            // Pixel padding
            byte[] paddedBitmap = paddingService.padBitmap(bitmapWithoutHeader, width, height);
            int paddedWidth = width + 2;
            int paddedHeight = height + 2;

            // Split data for threads
            List<ConversionTask> conversionTasks = ConversionTask.splitForTasks(paddedWidth, paddedHeight, cores, bytesPerPixel);
            Thread[] threadArray = new Thread[cores];
            for (int i = 0; i < cores; i++)
            {
                int startY = conversionTasks[i].startY;
                int endY = conversionTasks[i].endY;
                byte[] output = conversionTasks[i].outputData;

                Thread thread = new Thread(() =>
                {
                    APPLY_FLITER(paddedBitmap, output, paddedWidth, paddedHeight, startY, endY);

                });
                threadArray[i] = thread;

            }

            // Run tasks and perform time measurement
            long timeStart = Stopwatch.GetTimestamp();
            for (int i = 0; i < cores; i++)
            {
                threadArray[i].Start();
            }
            for (int i = 0; i < cores; i++)
            {
                threadArray[i].Join();
            }
            long timeEnd = Stopwatch.GetTimestamp();


            // Concatenate processed chunks of bitmap
            byte[] concatenatedChunks = Array.Empty<byte>();
            conversionTasks.OrderBy(task => task.orderIndex).ToList().ForEach(task =>
            {
                concatenatedChunks = concatenatedChunks.Concat(task.outputData).ToArray();
            });

            // Remove padding and attach header
            byte[] concatenatedChunksWithoutPadding = paddingService.removePadding(concatenatedChunks, paddedWidth, paddedHeight);
            byte[] outputBitmap = new byte[bitmapFromFile.Length];
            bitmapHeader.CopyTo(outputBitmap, 0);
            Array.Copy(concatenatedChunksWithoutPadding, 0, outputBitmap, dataOffset, concatenatedChunksWithoutPadding.Length);

            return timeEnd - timeStart;
        }



    }
}
