using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Twilio.Clients;

namespace CustomClientDotNetCore
{
    public class ProxiedTwilioClientCreator
    {
        private readonly IConfiguration _configuration;
        private static HttpClient _httpClient;

        public ProxiedTwilioClientCreator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void CreateHttpClient()
        {
            var proxyUrl = _configuration["ProxyServerUrl"];
            var handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(proxyUrl),
                UseProxy = true
            };

            _httpClient = new HttpClient(handler);
            var byteArray = Encoding.Unicode.GetBytes(
                _configuration["ProxyUsername"] + ":" +
                _configuration["ProxyPassword"]
            );

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(byteArray));
        }

        public TwilioRestClient GetClient()
        {
            var accountSid = _configuration["TwilioAccountSid"];
            var authToken = _configuration["TwilioAuthToken"];

            if (_httpClient == null)
            {
                // It's best* to create a single HttpClient and reuse it
                // * See: https://goo.gl/FShAAe
                CreateHttpClient();
            }

            var twilioRestClient = new TwilioRestClient(
                accountSid,
                authToken,
                httpClient: new Twilio.Http.SystemNetHttpClient(_httpClient)
            );

            return twilioRestClient;
        }
    }
}
