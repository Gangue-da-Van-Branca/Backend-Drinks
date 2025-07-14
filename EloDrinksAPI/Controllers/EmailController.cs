using EloDrinksAPI.DTOs.email;
using EloDrinksAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EloDrinksAPI.Controllers
{
    /// <summary>
    /// Endpoint para envio de e-mails genéricos.
    /// </summary>
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

        /// <summary>
        /// Envia um e-mail através do serviço configurado.
        /// </summary>
        /// <param name="request">Dados do e-mail (destinatário, assunto, corpo).</param>
        /// <response code="200">E-mail enviado com sucesso.</response>
        /// <response code="400">Dados da requisição inválidos.</response>
        /// <response code="500">Ocorreu um erro interno ao tentar enviar o e-mail.</response>
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