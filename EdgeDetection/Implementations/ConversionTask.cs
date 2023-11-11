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
        public int startY;
        public int endY;
        public byte[] outputData;

        public static List<ConversionTask> splitForTasks(int width, int height, int threads, int bytesPerPixel)
        {
            List<ConversionTask> conversionTasks = new List<ConversionTask>();

            int rowsPerThread = height / threads;
            int remainder = height % threads;

            int startRow = 0;
            int endRow;

            for (int i = 0; i < threads; i++)
            {
                endRow = startRow + rowsPerThread - 1;

                // If the number of rows is not divisible by the number of threads, we divide it without remainder and split it equaly (+1 for almost each row)
                if (remainder > 0)
                {
                    endRow++;
                    remainder--;
                }

                ConversionTask task = new ConversionTask(i, startRow, endRow, new byte[(endRow - startRow + 1) * width * bytesPerPixel]);
                conversionTasks.Add(task);
                startRow = endRow + 1;
            }

            return conversionTasks;
        }
        public ConversionTask(int orderIndex, int startY, int endY, byte[] outputData)
        {
            this.orderIndex = orderIndex;
            this.startY = startY;
            this.endY = endY;
            this.outputData = outputData;
        }
    }
}
