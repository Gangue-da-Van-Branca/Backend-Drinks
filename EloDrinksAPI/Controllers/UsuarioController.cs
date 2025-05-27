using EloDrinksAPI.DTOs.usuario;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EloDrinksAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ElodrinkContext _context;

        public UsuarioController(ElodrinkContext context)
        {
            _context = context;
        }

        // GET: /Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                return Ok(usuarios.Select(UsuarioMapper.ToDTO));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar usuários: {ex.Message}");
            }
        }

        // GET: /Usuario/id
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(string id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                    return NotFound("Usuário não encontrado");

                return Ok(UsuarioMapper.ToDTO(usuario));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar usuário: {ex.Message}");
            }
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> PostUsuario([FromBody] CreateUsuarioDto dto)
        {
            try
            {
                var usuario = UsuarioMapper.ToEntity(dto);

                usuario.DataCadastro = DateOnly.FromDateTime(DateTime.Today);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var usuarioResponse = UsuarioMapper.ToDTO(usuario);
                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuarioResponse);
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                return StatusCode(500, $"Erro ao criar usuário: {ex.Message} - Inner: {inner}");
            }

        }

        // PUT: /Usuario/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(string id, [FromBody] UpdateUsuarioDto dto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                    return NotFound("Usuário não encontrado");

                UsuarioMapper.ApplyUpdate(dto, usuario);

                await _context.SaveChangesAsync();
                return Ok("Usuário atualizado com sucesso.");
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, "Conflito de atualização no banco de dados.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        // DELETE: /Usuario/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                    return NotFound("Usuário não encontrado.");

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return Ok("Usuário deletado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar usuário: {ex.Message}");
            }
        }
    }
}
