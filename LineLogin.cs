using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace LineApi
{
    public class LineLogin : BaseApi
    {
        public LineLogin(IConfiguration config)
        {
            var section = config.GetSection("LineChannel");
            this.Initialize(
                section.GetValue<string>("ChannelId"),
                section.GetValue<string>("ChannelSecret"),
                section.GetValue<string>("StateKey"));
        }

        public string GetLoginAuthUrl(HttpRequest request, string responsePath)
        {
            return this.GetAuthUrl(request, responsePath, "https://access.line.me/oauth2/v2.1/authorize", "profile");
        }

        public OAuthResponse GetOAuthToken(HttpRequest request, string code, string responsePath)
        {
            return base.GetOAuthToken(request, code, responsePath, "https://api.line.me/oauth2/v2.1/token");
        }

        public LineProfile GetProfile(string token)
        {
            using HttpClient client = new HttpClient(GetHandler());
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage msg = client.GetAsync("https://api.line.me/v2/profile").Result;
            return msg.Content.ReadAsAsync<LineProfile>().Result;
        }

        public class LineProfile 
        {
            public string UserId { get; set; }

            public string DisplayName { get; set; }
        }
    }
}
