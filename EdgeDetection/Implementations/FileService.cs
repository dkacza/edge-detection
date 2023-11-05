using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdgeDetection.Implementations
{
    internal class FileService
    {
        public byte[] LoadImageAsBytes(string imagePath)
        {
            try
            {
                // Check if the file exists
                if (File.Exists(imagePath))
                {
                    // Read the image file into a byte array
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    return imageBytes;
                }
                else
                {
                    return Array.Empty<byte>();
                }
            }
            catch (Exception ex)
            {
                return Array.Empty<byte>();
            }
        }

        public void SaveByteArray(string outputPath, byte[] data)
        {
            File.WriteAllBytes(outputPath, data);
        }
    }
}
