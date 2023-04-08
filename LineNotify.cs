using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace LineApi
{
    public class LineNotify : BaseApi
    {
        public LineNotify(IConfiguration config)
        {
            var section = config.GetSection("LineNotify");
            this.Initialize(
                section.GetValue<string>("ClientId"),
                section.GetValue<string>("ClientSecret"),
                section.GetValue<string>("StateKey"));
        }

        public string GetNotifyAuthUrl(HttpRequest request, string responsePath)
        {
            return this.GetAuthUrl(request, responsePath, "https://notify-bot.line.me/oauth/authorize", "notify");
        }

        public OAuthResponse GetOAuthToken(HttpRequest request,string code, string responsePath)
        {
            return base.GetOAuthToken(request, code, responsePath, "https://notify-bot.line.me/oauth/token");
        }

        public BasicResponse SendMessage(string token, string message)
        {
            using HttpClient client = new HttpClient(GetHandler());
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            List<KeyValuePair<string, string>> colle = new List<KeyValuePair<string, string>>();
            colle.Add(new KeyValuePair<string, string>("message", message));

            using var content = new FormUrlEncodedContent(colle);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            
            HttpResponseMessage msg = client.PostAsync("https://notify-api.line.me/api/notify", content).Result;
            return msg.Content.ReadAsAsync<BasicResponse>().Result;
        }

        public BasicResponse RevokeToken(string token)
        {
            using HttpClient client = new HttpClient(GetHandler());
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>());
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            HttpResponseMessage msg = client.PostAsync("https://notify-api.line.me/api/revoke", content).Result;
            return msg.Content.ReadAsAsync<BasicResponse>().Result;
        }

        public class BasicResponse
        {
            [JsonProperty("status")]
            public int Status { get; set; }
        }
    }
}
