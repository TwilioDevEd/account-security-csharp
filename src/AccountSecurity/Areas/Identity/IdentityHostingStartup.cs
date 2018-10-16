using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AccountSecurity.Models;

[assembly: HostingStartup(typeof(AccountSecurity.Areas.Identity.IdentityHostingStartup))]
namespace AccountSecurity.Areas.Identity
{

    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AccountSecurityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddDefaultIdentity<ApplicationUser>(config =>
                    {
                        config.SignIn.RequireConfirmedEmail = false;
                    })
                    .AddEntityFrameworkStores<AccountSecurityContext>();
            });
        }
    }
}