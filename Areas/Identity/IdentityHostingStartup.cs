using Edge.Data;
using Edge.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Edge.Areas.Identity.IdentityHostingStartup))]
namespace Edge.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("EdgeContextConnection")));
                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
                
                //services.AddDefaultIdentity<ApplicationUser>(options =>
                //{
                //    options.SignIn.RequireConfirmedAccount = true;
                //    options.User.RequireUniqueEmail = false;
                //})
                //.AddEntityFrameworkStores<ApplicationDbContext>();
                // services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                                .AddRoles<IdentityRole>()
                                .AddEntityFrameworkStores<EdgeContext>()
            });
        }
    }
}