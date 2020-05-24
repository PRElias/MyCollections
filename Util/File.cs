using System.IO;
using Microsoft.AspNetCore.Http;

namespace MyCollections.Util
{
    public static class File
    {
        public static bool UploadFile(IFormFile ufile, string fileName)
        {
            if (ufile != null && ufile.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"docs\games\covers", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ufile.CopyTo(fileStream);
                }
                return true;
            }
            return false;
        }
    }
}