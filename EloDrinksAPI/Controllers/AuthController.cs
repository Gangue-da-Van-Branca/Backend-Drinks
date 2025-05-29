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

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                role = role  // adiciona o tipo do usuário aqui
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Ok("Se o e-mail estiver cadastrado, enviaremos instruções para redefinir a senha.");

            var token = Guid.NewGuid().ToString(); // <= geracao do token

            var resetToken = new PasswordResetToken
            {
                Token = token,
                UserId = user.IdUsuario,
                Expiration = DateTime.UtcNow.AddMinutes(30)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // ============== IMPORTANTE ==============
            // Link de redefinição de senha, vai ter que mudar qnd tiver deployado
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

            return Ok("Se o e-mail estiver cadastrado, enviaremos instruções para redefinir a senha.");
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