using EloDrinksAPI.DTOs.email;
using EloDrinksAPI.Services;
using Microsoft.AspNetCore.Mvc;


namespace EloDrinksAPI.Controllers
{

    [ApiController]
    [Route("email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("mensagem")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _emailService.SendEmailAsync(request);
                return Ok(new { Success = true, Message = $"E-mail enviado para {request.To}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail.");
                return StatusCode(500, new { Success = false, Error = ex.Message });
            }
        }
    }

}