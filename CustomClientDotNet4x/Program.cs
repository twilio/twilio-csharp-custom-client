using System;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CustomClientDotNet4x
{
    class Program
    {
        static void Main(string[] args)
        {
            var twilioRestClient = ProxiedTwilioClientCreator.GetClient();

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
