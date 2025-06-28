using EloDrinksAPI.DTOs.usuario;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EloDrinksAPI.Controllers
{
    /// <summary>
    /// Gerencia os dados dos usuários.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ElodrinkContext _context;

        public UsuarioController(ElodrinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todos os usuários do sistema (Apenas Admin).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDto>), 200)]
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

        /// <summary>
        /// Busca um usuário específico pelo seu ID (Requer autenticação).
        /// </summary>
        /// <param name="id">O ID do usuário.</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        [ProducesResponseType(typeof(UsuarioResponseDto), 200)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Atualiza os dados de um usuário (Requer autenticação).
        /// </summary>
        /// <remarks>
        /// Um usuário pode atualizar seus próprios dados. Um admin pode atualizar os dados de qualquer usuário.
        /// </remarks>
        /// <param name="id">O ID do usuário a ser atualizado.</param>
        /// <param name="dto">Os novos dados para o usuário.</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "user,admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Exclui um usuário do sistema (Apenas Admin).
        /// </summary>
        /// <param name="id">O ID do usuário a ser excluído.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
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