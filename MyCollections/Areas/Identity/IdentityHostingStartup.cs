using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCollections.Models;

[assembly: HostingStartup(typeof(MyCollections.Areas.Identity.IdentityHostingStartup))]
namespace MyCollections.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MyCollectionsContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddDefaultIdentity<User>()
                    .AddEntityFrameworkStores<MyCollectionsContext>();
            });
        }
    }
}