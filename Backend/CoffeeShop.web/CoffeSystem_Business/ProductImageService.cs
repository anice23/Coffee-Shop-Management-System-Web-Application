using System;
using System.Drawing;
using System.IO;
namespace CoffeSystem_Business
{
    public class ProductImageService
    {
        private readonly string _imagesFolder;

        public ProductImageService(string imagesFolder)
        {
            _imagesFolder = imagesFolder;
            Directory.CreateDirectory(_imagesFolder);

        }

        public string SaveImage(string sourcePath)
        {


            string extension = Path.GetExtension(sourcePath);

            string fileName =
                $"{Guid.NewGuid()}{extension}";

            string destinationPath = Path.Combine(
                _imagesFolder,
                fileName
            );

            File.Copy(
                sourcePath,
                destinationPath
            );

            return fileName;
        }
    }
}
       
   