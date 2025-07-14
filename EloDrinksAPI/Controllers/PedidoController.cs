using EloDrinksAPI.DTOs.pedido;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace EloDrinksAPI.Controllers
{
    /// <summary>
    /// Gerencia os pedidos gerados a partir dos orçamentos.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly ElodrinkContext _context;

        public PedidoController(ElodrinkContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todos os pedidos do sistema.
        /// </summary>
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

        /// <summary>
        /// Busca um pedido específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do pedido.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoResponseDto>> GetPedido(string id)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .FirstOrDefaultAsync(p => p.IdPedido == id);

                if (pedido == null)
                    return NotFound("Pedido não encontrado.");

                return PedidoMapper.ToDTO(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar pedido: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um novo pedido manualmente.
        /// </summary>
        /// <remarks>
        /// A criação de pedidos pelo cliente é feita automaticamente via OrçamentoController.
        /// </remarks>
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

        /// <summary>
        /// Atualiza um pedido existente.
        /// </summary>
        [HttpPut("{idPedido}/{idOrcamento}/{idUsuario}")]
        public async Task<IActionResult> PutPedido(string idPedido, string idOrcamento, string idUsuario, [FromBody] UpdatePedidoDto dto)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(idPedido, idOrcamento, idUsuario);
                if (pedido == null)
                    return NotFound("Pedido não encontrado.");

                PedidoMapper.ApplyUpdate(dto, pedido);
                await _context.SaveChangesAsync();

                return Ok("Pedido atualizado com sucesso.");
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

        /// <summary>
        /// Deleta um pedido.
        /// </summary>
        [HttpDelete("{idPedido}/{orcamentoId}/{usuarioId}")]
        public async Task<IActionResult> DeletePedido(string idPedido, string orcamentoId, string usuarioId)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(idPedido, orcamentoId, usuarioId);
                if (pedido == null)
                    return NotFound("Pedido não encontrado.");

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();

                return Ok("Pedido deletado com sucesso.");
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

        /// <summary>
        /// Busca todos os pedidos de um usuário específico.
        /// </summary>
        /// <param name="id">O ID do usuário.</param>
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