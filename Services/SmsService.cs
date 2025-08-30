using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;

namespace EcbMartService.Services
{
public interface ISmsSender
{
    Task SendSmsAsync(string number, string message);
}

public class TwilioSmsSender : ISmsSender
{
    private readonly IConfiguration _config;

    public TwilioSmsSender(IConfiguration config)
    {
        _config = config;
        TwilioClient.Init(
            _config["Twilio:AccountSid"], 
            _config["Twilio:AuthToken"]);
    }

    public Task SendSmsAsync(string number, string message)
    {
        return MessageResource.CreateAsync(
            to: new Twilio.Types.PhoneNumber(number),
            from: new Twilio.Types.PhoneNumber(_config["Twilio:FromNumber"]),
            body: message
        );
    }
}
}
