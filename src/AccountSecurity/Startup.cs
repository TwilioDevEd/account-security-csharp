using AccountSecurity.Models;
using AccountSecurity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;
using System;

namespace AccountSecurity {
    public class Startup {
        public IConfiguration Configuration { get; }

        private readonly ILogger<UserController> logger;

        public Startup(IHostingEnvironment env, IConfiguration config, ILoggerFactory loggerFactory) {
            logger = loggerFactory.CreateLogger<UserController>();
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(opts=>{
                    opts.DefaultScheme = IdentityConstants.ApplicationScheme;
                })
                .AddIdentityCookies(opts =>{
                    // This would be empty typically, just demonstrating the new API per cookie
                    opts.ApplicationCookie.Configure(c => {
                        c.Cookie.Name = "MainCookie";
                        c.LoginPath = "/login";
                        c.LogoutPath = "/logout";
                    });
                });

            services.AddDistributedMemoryCache();

            services.AddSession(opts => {
                opts.Cookie.IsEssential = true;
                opts.IdleTimeout = TimeSpan.FromMinutes(5);
                opts.Cookie.HttpOnly = true;
            });

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AccountSecurityContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddDbContext<AccountSecurityContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("AuthyTwoFactor", policy =>
                                policy.RequireClaim(ClaimTypes.AuthenticationMethod, "TokenVerification"));
            });

            services.AddHttpClient();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<IAuthy, Authy>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}