using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EdgeDetection.Implementations
{
    public class CSharpImplementation : IConverter
    {
        private static string relativePath = "..\\..\\..\\..\\..\\CsLibrary\\bin\\x64\\Release\\CSLibrary.dll";
        public long Convert(string inputPath, string outputPath, int cores)
        {
            // Loading DLL dynamicaly
            MethodInfo? convertedMethod;
            try
            {
                var dllPath = Path.GetFullPath(relativePath);
                var assembly = Assembly.LoadFile(dllPath);
                var type = assembly.GetType("CsLibrary.CsEdgeDetection");
                convertedMethod = type.GetMethod("convert");
            } 
            catch
            {
                return 0;
            }


            // Instatiate services
            FileService fileService = new FileService();
            PaddingService paddingService = new PaddingService();

            // Load image
            // If the bitmap is empty, show message and return immeadiately
            byte[] bitmapFromFile = fileService.LoadImageAsBytes(inputPath);
            if (bitmapFromFile.Length == 0 ) 
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

            // Convert to grayscale
            byte[] grayscaleData = new byte[width * height];
            grayscaleData = GrayscaleService.ConvertToGrayscale(bitmapWithoutHeader, grayscaleData, width, height);


            // Pixel padding
            byte[] paddedBitmap = paddingService.padBitmap(grayscaleData, width, height);
            int paddedWidth = width + 2;
            int paddedHeight = height + 2;

            // Split data for threads
            List<ConversionTask> conversionTasks = ConversionTask.splitForTasks(paddedWidth, paddedHeight, cores);
            Thread[] threadArray = new Thread[cores];
            for (int i = 0; i < cores; i++)
            {
                int startY = conversionTasks[i].startY;
                int endY = conversionTasks[i].endY;
                byte[] output = conversionTasks[i].outputData;

                Thread thread = new Thread(() =>
                {
                    convertedMethod.Invoke(null, new object[] { paddedBitmap, output, paddedWidth, paddedHeight, startY, endY });
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

            // Restore grayscale to RGB
            byte[] outputBitmap = new byte[bitmapFromFile.Length];
            for (int i = 0; i < concatenatedChunksWithoutPadding.Length; i++)
            {
                outputBitmap[i * 3 + dataOffset] = concatenatedChunksWithoutPadding[i];
                outputBitmap[(i * 3) + 1 + dataOffset] = concatenatedChunksWithoutPadding[i];
                outputBitmap[(i * 3) + 2 + dataOffset] = concatenatedChunksWithoutPadding[i];
            }

            bitmapHeader.CopyTo(outputBitmap, 0);


            // Save to file
            fileService.SaveByteArray(outputPath, outputBitmap);

            return timeEnd - timeStart;

        }

        public long Measure(string inputPath, int cores)
        {
            // Loading DLL dynamicaly
            MethodInfo? convertedMethod;
            try
            {
                var dllPath = Path.GetFullPath(relativePath);
                var assembly = Assembly.LoadFile(dllPath);
                var type = assembly.GetType("CsLibrary.CsEdgeDetection");
                convertedMethod = type.GetMethod("convert");
            }
            catch
            {
                return 0;
            }


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

            // Convert to grayscale
            byte[] grayscaleData = new byte[width * height];
            grayscaleData = GrayscaleService.ConvertToGrayscale(bitmapWithoutHeader, grayscaleData, width, height);


            // Pixel padding
            byte[] paddedBitmap = paddingService.padBitmap(grayscaleData, width, height);
            int paddedWidth = width + 2;
            int paddedHeight = height + 2;

            // Split data for threads
            List<ConversionTask> conversionTasks = ConversionTask.splitForTasks(paddedWidth, paddedHeight, cores);
            Thread[] threadArray = new Thread[cores];
            for (int i = 0; i < cores; i++)
            {
                int startY = conversionTasks[i].startY;
                int endY = conversionTasks[i].endY;
                byte[] output = conversionTasks[i].outputData;

                Thread thread = new Thread(() =>
                {
                    convertedMethod.Invoke(null, new object[] { paddedBitmap, output, paddedWidth, paddedHeight, startY, endY });
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

            // Restore grayscale to RGB
            byte[] outputBitmap = new byte[bitmapFromFile.Length];
            for (int i = 0; i < concatenatedChunksWithoutPadding.Length; i++)
            {
                outputBitmap[i * 3 + dataOffset] = concatenatedChunksWithoutPadding[i];
                outputBitmap[(i * 3) + 1 + dataOffset] = concatenatedChunksWithoutPadding[i];
                outputBitmap[(i * 3) + 2 + dataOffset] = concatenatedChunksWithoutPadding[i];
            }
            bitmapHeader.CopyTo(outputBitmap, 0);

            return timeEnd - timeStart;
        }
    }
}
