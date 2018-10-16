using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AccountSecurity.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AccountSecurity.Services {
    public class Authy {
        private readonly IConfiguration Configuration;
        private readonly IHttpClientFactory ClientFactory;

        public HttpClient client { get; }

        private readonly ILogger<UserController> logger;

        public Authy(IConfiguration config, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory) {
            Configuration = config;
            ClientFactory = clientFactory;
            client = ClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.authy.com/protected/json");
            logger = loggerFactory.CreateLogger<UserController>();

        }

        private HttpRequestMessage NewRequest(string uri, HttpMethod method)
        {
            var request = new HttpRequestMessage(
                method,
                uri
            );
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "Twilio Account Security C# Sample");

            // Get Authy API Key from Configuration
            request.Headers.Add("X-Authy-API-Key", Configuration["AuthyApiKey"]);
            return request;
        }

        private HttpRequestMessage NewRequest(string uri)
        {
            return NewRequest(uri, HttpMethod.Get);
        }

        public async Task<Dictionary<string, string>> registerUserAsync(ApplicationUser user)
        {
            var result = await client.PostAsJsonAsync("/user/new", new Dictionary<string, string>()
            {
                {"email", user.Email},
                // {"country_code", user.CountryCode},
                {"cellphone", user.PhoneNumber} 
            });

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadAsAsync<Dictionary<string, string>>();
        }

        async Task sendSmsAsync() {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.authy.com/protected/json/sms/$AUTHY_ID?force=true"
            );
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "Twilio AccountSecurity C# Sample");

            // Get Authy API Key from Configuration
            request.Headers.Add("X-Authy-API-Key", Configuration["AuthyApiKey"]);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}