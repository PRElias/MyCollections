using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MyCollections
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://localhost:5000", "http://*:5000");
                    webBuilder.UseStartup<Startup>();
                });
        }
}
