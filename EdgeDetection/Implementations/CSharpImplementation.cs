using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CsLibrary;

namespace EdgeDetection.Implementations
{
    public class CSharpImplementation : IConverter
    {
        public long Convert(string inputPath, string outputPath, int cores, int threshold)
        {
            // Loading DLL dynamicaly
            MethodInfo? convertedMethod;
            try
            {
                var dllPath = "D:\\Dev\\CS\\edge-detection\\CsLibrary\\bin\\Debug\\CSLibrary.dll";
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


            // Pixel padding
            byte[] paddedBitmap = paddingService.padBitmap(bitmapWithoutHeader, width, height);
            int paddedWidth = width + 2;
            int paddedHeight = height + 2;

            // Split data for threads
            List<ConversionTask> conversionTasks = ConversionTask.splitForTasks(paddedHeight, paddedWidth, cores, bytesPerPixel);
            Thread[] threadArray = new Thread[cores];
            for (int i = 0; i < cores; i++)
            {
                int startIndexArg = conversionTasks[i].startIndex;
                byte[] outputArg = conversionTasks[i].outputData;
                int pixelsToProcessArg = conversionTasks[i].pixelsToProcess;

                Thread thread = new Thread(() =>
                {
                    //CsEdgeDetection.convert(paddedBitmap, startIndexArg, outputArg, pixelsToProcessArg);
                    convertedMethod.Invoke(null, new object[] { paddedBitmap, startIndexArg, outputArg, pixelsToProcessArg });
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
    }
}
