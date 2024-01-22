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

        public static List<ConversionTask> splitForTasks(int width, int height, int threads)
        {
            List<ConversionTask> conversionTasks = new List<ConversionTask>();

            int rowsPerThread = height / threads;
            int remainder = height % threads;

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

                ConversionTask task = new ConversionTask(i, startRow, endRow, new byte[(endRow - startRow + 1) * width]);
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
