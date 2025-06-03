using EloDrinksAPI.DTOs.pedido;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Services;

namespace EloDrinksAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly ElodrinkContext _context;

        public PedidoController(ElodrinkContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoResponseDto>>> GetPedidos()
        {
            try
            {
                var pedidos = await _context.Pedidos.ToListAsync();
                return pedidos.Select(PedidoMapper.ToDTO).ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar pedidos: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoResponseDto>> GetPedido(string id)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(id);
                if (pedido == null)
                    return NotFound("Pedido não encontrado.");

                return PedidoMapper.ToDTO(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar pedido: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PedidoResponseDto>> PostPedido([FromBody] CreatePedidoDto dto)
        {
            try
            {
                var pedido = PedidoMapper.ToEntity(dto);
                pedido.IdPedido = "p1" + GerarIdService.GerarIdAlfanumerico(16);
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                var response = PedidoMapper.ToDTO(pedido);
                return CreatedAtAction(nameof(GetPedido), new { id = pedido.IdPedido }, response);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Erro ao criar pedido no banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado ao criar pedido: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(string id, [FromBody] UpdatePedidoDto dto)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(id);
                if (pedido == null)
                    return NotFound("Pedido não encontrado para atualização.");

                PedidoMapper.ApplyUpdate(dto, pedido);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Erro ao atualizar pedido no banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado ao atualizar pedido: {ex.Message}");
            }
        }

        [HttpDelete("{idPedido}/{orcamentoId}/{usuarioId}")]
        public async Task<IActionResult> DeletePedido(string idPedido, string orcamentoId, string usuarioId)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(idPedido, orcamentoId, usuarioId);
                if (pedido == null)
                    return NotFound("Pedido não encontrado para exclusão.");

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();

                return Ok(pedido);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Erro ao excluir pedido no banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado ao excluir pedido: {ex.Message}");
            }
        }

        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<IEnumerable<PedidoResponseDto>>> GetPedidosByUsuario(string id)
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Where(p => p.OrcamentoUsuarioIdUsuario == id)
                    .ToListAsync();

                if (pedidos == null || !pedidos.Any())
                    return NotFound("Nenhum pedido encontrado para o usuário informado.");

                return pedidos.Select(PedidoMapper.ToDTO).ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar pedidos por usuário: {ex.Message}");
            }
        }
    }
}
