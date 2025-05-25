using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.DTOs.orcamento;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Authorization;
using EloDrinksAPI.Services;

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

    // GET: api/Orcamento
    [Authorize(Roles = "admin")]
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
    
    // GET: api/Orcamento/5
    [Authorize(Roles = "admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrcamentoResponseDto>> GetOrcamento(string id)
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

    // POST: api/Orcamento
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult<OrcamentoResponseDto>> PostOrcamento(CreateOrcamentoDto dto)
    {
        try
        {
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.UsuarioIdUsuario);
            if (!usuarioExiste)
                return BadRequest("Usuário não encontrado.");

            var entity = OrcamentoMapper.ToEntity(dto);
            entity.IdOrcamento = "o1" + GerarIdService.GerarIdAlfanumerico(16);
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

    // PUT: api/Orcamento/5
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrcamento(string id, UpdateOrcamentoDto dto)
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

    // DELETE: api/Orcamento/5
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrcamento(string id)
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
