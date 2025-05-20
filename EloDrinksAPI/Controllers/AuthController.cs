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
[Route("api/[controller]")]
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
public IActionResult Login([FromBody] LoginModel login)
{
    var user = _context.Usuarios.SingleOrDefault(x => x.Email == login.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(login.Senha, user.Senha))
        return Unauthorized("Credenciais inválidas.");

    // Verificar se a chave JWT está configurada
    var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key");
    if (string.IsNullOrEmpty(jwtKey))
        throw new InvalidOperationException("A chave JWT não foi configurada.");

    var key = Encoding.ASCII.GetBytes(jwtKey);

    // USAR ENQNT O 'TIPO' DO BANCO NAO FOR ENUM
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

}

}