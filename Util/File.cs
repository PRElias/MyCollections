using System.IO;
using System.Linq;
using System;
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

        public static async Task<string> DownloadImageFromUrlAsync(string url, string baseFileName, string referer = null)
        {
            using HttpClient client = CreateImageHttpClient();
            using HttpRequestMessage request = CreateImageRequest(url, referer);
            using HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (String.IsNullOrWhiteSpace(mediaType) || !mediaType.StartsWith("image/"))
            {
                throw new HttpRequestException("A URL escolhida não retornou uma imagem.");
            }

            var extension = GetImageExtension(url, mediaType);
            var fileName = baseFileName + extension;
            await SaveImageResponseAsync(response, fileName);
            return fileName;
        }

        private static HttpClient CreateImageHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
            return client;
        }

        private static HttpRequestMessage CreateImageRequest(string url, string referer)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.ParseAdd("image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");

            if (!String.IsNullOrWhiteSpace(referer) && Uri.TryCreate(referer, UriKind.Absolute, out var refererUri))
            {
                request.Headers.Referrer = refererUri;
            }

            return request;
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
