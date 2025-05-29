using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using EloDrinksAPI.DTOs.usuario;
using EloDrinksAPI.Services;
using EloDrinksAPI.DTOs.email;
using EloDrinksAPI.DTOs.forgotPassword;

namespace EloDrinksAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ElodrinkContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthController(ElodrinkContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUsuarioDto usuarioDTO)
        {
            if (_context.Usuarios.Any(u => u.Email == usuarioDTO.Email))
                return BadRequest("Usuário já existe.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha);

            var usuario = new Usuario
            {
                IdUsuario = "u1" + GerarIdService.GerarIdAlfanumerico(16),
                Nome = usuarioDTO.Nome,
                Sobrenome = usuarioDTO.Sobrenome,
                Email = usuarioDTO.Email,
                Telefone = usuarioDTO.Telefone,
                Senha = hashedPassword,
                Tipo = usuarioDTO.Tipo,
                DataCadastro = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuário registrado com sucesso.");
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Usuarios.SingleOrDefault(x => x.Email == login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Senha, user.Senha))
                return Unauthorized("Credenciais inválidas.");

            // verifica se a chave JWT está configurada
            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key");
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("A chave JWT não foi configurada.");

            var key = Encoding.ASCII.GetBytes(jwtKey);

            string role = user.Tipo == "1" ? "admin" : "user";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role) // role convertida vem pra ca
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                // Tenta encontrar o usuário pelo e-mail fornecido.
                var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

                // Por segurança, a mesma mensagem é retornada existindo ou não o usuário.
                // Isso evita que um atacante use este endpoint para descobrir quais e-mails estão cadastrados.
                if (user == null)
                    return Ok("Se o e-mail estiver cadastrado, enviaremos instruções para redefinir a senha.");

                // Geração de um token único para a redefinição.
                var token = Guid.NewGuid().ToString();

                var resetToken = new PasswordResetToken
                {
                    Token = token,
                    UserId = user.IdUsuario,
                    Expiration = DateTime.UtcNow.AddMinutes(30) // Token expira em 30 minutos.
                };

                // Adiciona e salva o token no banco de dados.
                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                // Monta o link que será enviado ao usuário.
                // TODO: Substituir "https://seudominio.com" pela URL do frontend quando estiver em produção.
                var resetLink = $"https://seudominio.com/reset-password?token={token}";

                // Prepara o corpo do e-mail.
                var emailRequest = new EmailRequestDto
                {
                    To = user.Email,
                    Subject = "Redefinição de Senha - EloDrinks",
                    Body = $@"
        <p>Olá {user.Nome},</p>
        <p>Você solicitou uma redefinição de senha.</p>
        <p>Clique no link abaixo para criar uma nova senha. O link expira em 30 minutos:</p>
        <p><a href='{resetLink}'>Redefinir senha</a></p>
        <p>Se você não solicitou isso, ignore este e-mail.</p>
        <p>Atenciosamente,<br/>Equipe EloDrinks</p>"
                };

                // Envia o e-mail para o usuário.
                await _emailService.SendEmailAsync(emailRequest);

                // Retorna a mesma mensagem de sucesso para proteger a privacidade dos usuários.
                return Ok("Se o e-mail estiver cadastrado, enviaremos instruções para redefinir a senha.");
            }
            catch (Exception ex)
            {
                // Em caso de qualquer falha (ex: banco de dados offline, serviço de e-mail com problemas),
                // a exceção será capturada aqui.

                // É crucial logar a exceção para diagnóstico de problemas.
                // Ex: _logger.LogError(ex, "Ocorreu um erro ao processar a solicitação de redefinição de senha.");

                // Retorna um erro 500 (Internal Server Error) com uma mensagem genérica para o cliente.
                // Isso evita expor detalhes técnicos da falha.
                return StatusCode(500, $"Ocorreu uma falha interna ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var tokenEntry = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == dto.Token && t.Expiration > DateTime.UtcNow);

            if (tokenEntry == null)
                return BadRequest("Token inválido ou expirado.");

            var user = await _context.Usuarios.FindAsync(tokenEntry.UserId);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            user.Senha = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);

            _context.PasswordResetTokens.Remove(tokenEntry); // <= invalidacao do token
            await _context.SaveChangesAsync();

            return Ok("Senha redefinida com sucesso.");
        }


    }

}