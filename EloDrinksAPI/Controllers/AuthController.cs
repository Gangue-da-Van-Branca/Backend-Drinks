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

namespace EloDrinksAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ElodrinkContext _context;
        private readonly IConfiguration _config;

        public AuthController(ElodrinkContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
                return BadRequest("Usuário já existe.");

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            usuario.DataCadastro = DateOnly.FromDateTime(DateTime.UtcNow);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuário registrado com sucesso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var user = _context.Usuarios.Include(u => u.RefreshTokens).SingleOrDefault(x => x.Email == login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Senha, user.Senha))
                return Unauthorized("Credenciais inválidas.");

            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key");
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("A chave JWT não foi configurada.");

            var key = Encoding.ASCII.GetBytes(jwtKey);

            string role = user.Tipo == "1" ? "admin" : "user";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            // Criar e salvar o refresh token
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                Expiration = DateTime.UtcNow.AddDays(7),
                IdUsuario = user.IdUsuario
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = jwt,
                refreshToken = refreshToken.Token
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.Usuario)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || storedToken.Expiration < DateTime.UtcNow)
                return Unauthorized("Refresh token inválido ou expirado.");

            var user = storedToken.Usuario;
            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? "chave-padrao";
            var key = Encoding.ASCII.GetBytes(jwtKey);
            string role = user.Tipo == "1" ? "admin" : "user";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            // Regenerar refresh token (opcional, mas mais seguro)
            var newRefreshToken = GenerateRefreshToken();
            storedToken.Token = newRefreshToken;
            storedToken.Expiration = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = jwt,
                refreshToken = newRefreshToken
            });
        }


    }

}