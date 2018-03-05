using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using Twilio.Clients;

namespace CustomClientDotNet4x
{
    public static class ProxiedTwilioClientCreator
    {
        private static HttpClient _httpClient;

        private static void CreateHttpClient()
        {
            var proxyUrl = ConfigurationManager.AppSettings["ProxyServerUrl"];
            var handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(proxyUrl),
                UseProxy = true
            };

            _httpClient = new HttpClient(handler);
            var byteArray = Encoding.Unicode.GetBytes(
                ConfigurationManager.AppSettings["ProxyUsername"] + ":" +
                ConfigurationManager.AppSettings["ProxyPassword"]
            );

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(byteArray));
        }

        public static TwilioRestClient GetClient()
        {
            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];

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
