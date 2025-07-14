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
using Microsoft.AspNetCore.Authorization;

namespace EloDrinksAPI.Controllers
{
    /// <summary>
    /// Gerencia a autenticação, registro e recuperação de senha dos usuários.
    /// </summary>
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

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="usuarioDTO">Dados para a criação do novo usuário.</param>
        /// <response code="200">Usuário registrado com sucesso.</response>
        /// <response code="400">O e-mail fornecido já está em uso.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUsuarioDto usuarioDTO)
        {
            if (_context.Usuarios.Any(u => u.Email == usuarioDTO.Email))
                return BadRequest(new { erro = "Usuário já existe" });

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

            return Ok(new { message = "Usuário registrado com sucesso" });
        }


        /// <summary>
        /// Autentica um usuário e retorna um token JWT.
        /// </summary>
        /// <param name="login">Credenciais de login (email e senha).</param>
        /// <returns>Um token JWT, a role do usuário e seu ID.</returns>
        /// <response code="200">Login bem-sucedido. Retorna o token, role e ID do usuário.</response>
        /// <response code="401">Credenciais inválidas.</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Usuarios.SingleOrDefault(x => x.Email == login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Senha, user.Senha))
                return Unauthorized(new { erro = "Credenciais inválidas" });

            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key");
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("A chave JWT não foi configurada");

            var key = Encoding.ASCII.GetBytes(jwtKey);

            string role = user.Tipo == "1" ? "admin" : "user";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                role,
                user.IdUsuario
            });
        }

        /// <summary>
        /// Inicia o processo de recuperação de senha.
        /// </summary>
        /// <remarks>
        /// Sempre retorna uma mensagem de sucesso para não revelar se um e-mail está ou não cadastrado.
        /// </remarks>
        /// <param name="dto">Objeto contendo o e-mail para recuperação.</param>
        /// <response code="200">Instruções de recuperação enviadas (se o e-mail existir).</response>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Ok("Se o e-mail estiver cadastrado, enviaremos instruções para redefinir a senha");

            var token = Guid.NewGuid().ToString();

            var resetToken = new PasswordResetToken
            {
                Token = token,
                UserId = user.IdUsuario,
                Expiration = DateTime.UtcNow.AddMinutes(30)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var resetLink = $"https://seudominio.com/reset-password?token={token}";

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

            await _emailService.SendEmailAsync(emailRequest);

            return Ok("Se o e-mail estiver cadastrado, enviaremos instruções para redefinir a senha!");
        }

        /// <summary>
        /// Valida um token de recuperação de senha.
        /// </summary>
        /// <param name="token">O token recebido por e-mail.</param>
        /// <response code="200">O token é válido e pode ser usado para redefinir a senha.</response>
        /// <response code="400">O token é inválido ou já expirou.</response>
        [HttpGet("validate-resetPassword-token")]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
        {
            var tokenEntry = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.Expiration > DateTime.UtcNow);

            if (tokenEntry == null)
                return BadRequest("Token inválido ou expirado");

            return Ok("Token válido");
        }

        /// <summary>
        /// Redefine a senha do usuário utilizando um token válido.
        /// </summary>
        /// <param name="dto">Objeto contendo o token e a nova senha.</param>
        /// <response code="200">Senha redefinida com sucesso.</response>
        /// <response code="400">O token é inválido ou já expirou.</response>
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

            _context.PasswordResetTokens.Remove(tokenEntry);
            await _context.SaveChangesAsync();

            return Ok("Senha redefinida com sucesso.");
        }
    }
}