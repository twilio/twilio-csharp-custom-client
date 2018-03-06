using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CustomClientDotNetCore
{
    class Program
    {
        private static IConfiguration _configuration;

        static void Main(string[] args)
        {
            // Bootstrap configuration system. This is usually handled
            // for you in ASP.NET Core projects.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = builder.Build();

            // Create a custom Twilio client
            var clientCreator = new ProxiedTwilioClientCreator(_configuration);
            var twilioRestClient = clientCreator.GetClient();

            // Now that we have our custom built TwilioRestClient,
            // we can pass it to any REST API resource action.
            var message = MessageResource.Create(
                to: new PhoneNumber("+15017122661"),
                from: new PhoneNumber("+15017122661"),
                body: "Hey there!",
                // Here's where you inject the custom client
                client: twilioRestClient
            );

            Console.WriteLine($"Message SID: {message.Sid}");
        }
    }
}
