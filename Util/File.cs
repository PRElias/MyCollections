using System.IO;
using System.Linq;
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
            using HttpClient client = CreateImageHttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await SaveImageResponseAsync(response, fileName);
        }

        public static async Task<string> DownloadImageFromUrlAsync(string url, string baseFileName)
        {
            using HttpClient client = CreateImageHttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var extension = GetImageExtension(url, response.Content.Headers.ContentType?.MediaType);
            var fileName = baseFileName + extension;
            await SaveImageResponseAsync(response, fileName);
            return fileName;
        }

        private static HttpClient CreateImageHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 MyCollections/1.0");
            return client;
        }

        private static async Task SaveImageResponseAsync(HttpResponseMessage response, string fileName)
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), @"docs\games\covers", fileName);
            await using Stream stream = await response.Content.ReadAsStreamAsync();
            await using FileStream fileStream = new(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);
        }

        private static string GetImageExtension(string url, string mediaType)
        {
            return mediaType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => GetExtensionFromUrl(url)
            };
        }

        private static string GetExtensionFromUrl(string url)
        {
            var extension = Path.GetExtension(new System.Uri(url).AbsolutePath).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            return allowedExtensions.Contains(extension) ? extension : ".jpg";
        }
    }
}
