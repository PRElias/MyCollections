using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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

        public static async Task DownloadImageAsync(string url, string fileName)
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), @"docs\games\covers", fileName);
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using Stream stream = await response.Content.ReadAsStreamAsync();
            await using FileStream fileStream = new(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);
        }
    }
}