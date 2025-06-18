using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace BLL.Util
{
    public static class ImageHelper
    {
        public static async Task<string> UploadImageAsync(IFormFile imageFile, string uploadPath, string productName)
        {
            Console.WriteLine("Image Helper");
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return null;

                var cleanFileName = (productName + "_" + imageFile.FileName).Replace(" ", "_");
                var fullPath = Path.Combine(uploadPath, cleanFileName);


                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return cleanFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Image Helper Upload Image failed: " + ex.Message);
                throw new Exception("Image Helper Upload Image failed: " + ex.Message);
            }
                
        }
    }
}
