using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace LineApi
{
    public abstract class BaseApi
    {
        private string _clientId;
        private string _clientSecret;
        private string _stateKey;

        protected void Initialize(
            string clientId,
            string clientSecret,
            string stateKey)
        {
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._stateKey = stateKey;
        }

        public bool IsStateVaild(string state)
        {
            return BCrypt.Net.BCrypt.Verify(this._stateKey, state);
        }

        protected string GetAuthUrl(HttpRequest request, string responsePath, string url, string scope)
        {
            string redirectUrl = GetUrl(request, responsePath);
            string state = BCrypt.Net.BCrypt.HashString(this._stateKey);
            string fullUrl = $"{url}?" +
                   $"response_type=code&" +
                   $"client_id={this._clientId}&" +
                   $"redirect_uri={redirectUrl}&" +
                   $"scope={scope}&" +
                   $"state={state}";
            return fullUrl;
        }

        protected OAuthResponse GetOAuthToken(HttpRequest request, string code, string responsePath, string url)
        {
            using HttpClient client = new HttpClient(GetHandler());

            List<KeyValuePair<string, string>> colle = new List<KeyValuePair<string, string>>();
            colle.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));
            colle.Add(new KeyValuePair<string, string>("code", code));
            colle.Add(new KeyValuePair<string, string>("redirect_uri", GetUrl(request, responsePath)));
            colle.Add(new KeyValuePair<string, string>("client_id", this._clientId));
            colle.Add(new KeyValuePair<string, string>("client_secret", this._clientSecret));

            using var content = new FormUrlEncodedContent(colle);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            HttpResponseMessage msg = client.PostAsync(url, content).Result;
            return msg.Content.ReadAsAsync<OAuthResponse>().Result;
        }

        protected HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11;
            handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => { return true; };
            return handler;
        }

        private string GetUrl(HttpRequest request, string path)
        {
            path = string.IsNullOrWhiteSpace(path) || path[0] == '/' ? path : $"/{path}";
            string host = request.Host.Value;
            string http = host.Contains("localhost") || host.Contains("127.0.0.1") || host.Contains("192.168") ? "http://" : "https://";
            return $"{http}{request.Host.Value}{path}";
        }

        public class OAuthResponse
        {
            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
        }
    }
}
