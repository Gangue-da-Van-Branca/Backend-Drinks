using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.DTOs.orcamento;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.Models;

namespace EloDrinksAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class OrcamentoController : ControllerBase
{
    private readonly ElodrinkContext _context;

    public OrcamentoController(ElodrinkContext context)
    {
        _context = context;
    }

    // GET: /Orcamento
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrcamentoResponseDto>>> GetOrcamentos()
    {
        try
        {
            var orcamentos = await _context.Orcamentos
                .Include(o => o.UsuarioIdUsuarioNavigation)
                .ToListAsync();

            return orcamentos.Select(o => OrcamentoMapper.ToDTO(o)).ToList();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar orçamentos: {ex.Message}");
        }
    }

    // GET: /Orcamento/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OrcamentoResponseDto>> GetOrcamento(int id)
    {
        try
        {
            var orcamento = await _context.Orcamentos
                .Include(o => o.UsuarioIdUsuarioNavigation)
                .FirstOrDefaultAsync(o => o.IdOrcamento == id);

            if (orcamento == null)
                return NotFound();

            return OrcamentoMapper.ToDTO(orcamento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar orçamento: {ex.Message}");
        }
    }

    // POST: /Orcamento
    [HttpPost]
    public async Task<ActionResult<OrcamentoResponseDto>> PostOrcamento(CreateOrcamentoDto dto)
    {
        try
        {
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.UsuarioIdUsuario);
            if (!usuarioExiste)
                return BadRequest("Usuário não encontrado.");

            var entity = OrcamentoMapper.ToEntity(dto);
            _context.Orcamentos.Add(entity);
            await _context.SaveChangesAsync();

            var orcamentoCompleto = await _context.Orcamentos
                .Include(o => o.UsuarioIdUsuarioNavigation)
                .FirstOrDefaultAsync(o => o.IdOrcamento == entity.IdOrcamento);

            return CreatedAtAction(nameof(GetOrcamento), new { id = entity.IdOrcamento }, OrcamentoMapper.ToDTO(orcamentoCompleto!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao criar orçamento: {ex.Message}");
        }
    }

    // PUT: /Orcamento/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrcamento(int id, UpdateOrcamentoDto dto)
    {
        try
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
                return NotFound();

            OrcamentoMapper.ApplyUpdate(dto, orcamento);
            _context.Entry(orcamento).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar orçamento: {ex.Message}");
        }
    }

    // DELETE: /Orcamento/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrcamento(int id)
    {
        try
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
                return NotFound();

            _context.Orcamentos.Remove(orcamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao deletar orçamento: {ex.Message}");
        }
    }
}
