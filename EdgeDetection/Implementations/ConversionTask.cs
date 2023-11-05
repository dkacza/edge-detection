using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeDetection.Implementations
{
    internal class ConversionTask
    {
        public int orderIndex;
        public int startIndex;
        public int pixelsToProcess;
        public byte[] outputData;

        public static List<ConversionTask> splitForTasks(int rows, int columns, int threads, int bytesPerPixel)
        {
            List<ConversionTask> conversionTasks = new List<ConversionTask>();

            int rowsPerThread = rows / threads;
            int remainder = rows % threads;

            int startRow = 0;
            int endRow;

            for (int i = 0; i < threads; i++)
            {
                endRow = startRow + rowsPerThread - 1;
                if (remainder > 0)
                {
                    endRow++;
                    remainder--;
                }

                int pixelsToProcess = (endRow - startRow + 1) * columns;

                ConversionTask task = new ConversionTask(i, (startRow * columns) * bytesPerPixel, pixelsToProcess, new byte[pixelsToProcess * bytesPerPixel]);
                conversionTasks.Add(task);
                startRow = endRow + 1;
            }

            return conversionTasks;
        }
        public ConversionTask(int orderIndex, int startIndex, int pixelsToProcess, byte[] outputData)
        {
            this.orderIndex = orderIndex;
            this.startIndex = startIndex;
            this.pixelsToProcess = pixelsToProcess;
            this.outputData = outputData;
        }
    }
}
