namespace Kaypic_Web3.Services
{
    public interface ISMSSenderService
    {
        Task SendSmsAsync(string number, string message);
    }
}
