using AccountSecurity.Authorization;
using AccountSecurity.Models;
using AccountSecurity.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;

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
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                });

            // services.AddAuthorization(options => 
            // {
            //     options.AddPolicy("AuthyVerified", policy => 
            //     policy.Requirements.Add(new AuthyVerifiedRequirement()));
            // });

            services.AddHttpClient();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<IAuthy, Authy>();
            // services.AddSingleton<IAuthorizationHandler, AuthyVerifiedHandler>();
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
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}