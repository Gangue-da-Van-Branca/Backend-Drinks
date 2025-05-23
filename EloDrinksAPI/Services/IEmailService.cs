using EloDrinksAPI.DTOs.email;

namespace EloDrinksAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequestDto request);
    }
}
