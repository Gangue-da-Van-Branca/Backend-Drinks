using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Models;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.DTOs.orcamentoHasItem;

namespace EloDrinksAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class OrcamentoHasItemController : ControllerBase
{
    private readonly ElodrinkContext _context;

    public OrcamentoHasItemController(ElodrinkContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrcamentoHasItemResponseDto>>> GetAll()
    {
        try
        {
            var itens = await _context.OrcamentoHasItems
                .Include(o => o.ItemIdItemNavigation)
                .Select(o => OrcamentoHasItemMapper.ToDTO(o))
                .ToListAsync();

            return Ok(itens);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar dados: {ex.Message}");
        }
    }

    [HttpGet("{orcamentoId}/{itemId}")]
    public async Task<ActionResult<OrcamentoHasItemResponseDto>> Get(int orcamentoId, int itemId)
    {
        try
        {
            var ohi = await _context.OrcamentoHasItems
                .Include(o => o.ItemIdItemNavigation)
                .FirstOrDefaultAsync(o =>
                    o.OrcamentoIdOrcamento == orcamentoId &&
                    o.ItemIdItem == itemId);

            if (ohi == null)
                return NotFound();

            return Ok(OrcamentoHasItemMapper.ToDTO(ohi));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar item: {ex.Message}");
        }
    }

    [HttpGet("Orcamento/{orcamentoId}")]
    public async Task<ActionResult<IEnumerable<OrcamentoHasItemResponseDto>>> GetByOrcamento(int orcamentoId)
    {
        try
        {
            var itens = await _context.OrcamentoHasItems
                .Where(o => o.OrcamentoIdOrcamento == orcamentoId)
                .Include(o => o.ItemIdItemNavigation)
                .Select(o => OrcamentoHasItemMapper.ToDTO(o))
                .ToListAsync();

            return Ok(itens);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar itens do orçamento: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<OrcamentoHasItemResponseDto>> Post(CreateOrcamentoHasItemDto dto)
    {
        try
        {
            var orcamentoExiste = await _context.Orcamentos
                .AnyAsync(o => o.IdOrcamento == dto.OrcamentoIdOrcamento);
            var itemExiste = await _context.Items
                .AnyAsync(i => i.IdItem == dto.ItemIdItem);

            if (!orcamentoExiste || !itemExiste)
                return BadRequest("Orçamento ou Item não encontrado.");

            var existe = await _context.OrcamentoHasItems.AnyAsync(e =>
                e.OrcamentoIdOrcamento == dto.OrcamentoIdOrcamento &&
                e.ItemIdItem == dto.ItemIdItem);

            if (existe)
                return Conflict("Item já está associado a este orçamento.");

            var entity = OrcamentoHasItemMapper.ToEntity(dto);
            _context.OrcamentoHasItems.Add(entity);
            await _context.SaveChangesAsync();

            // Recarregar com navegação
            var carregado = await _context.OrcamentoHasItems
                .Include(o => o.ItemIdItemNavigation)
                .FirstAsync(o =>
                    o.OrcamentoIdOrcamento == dto.OrcamentoIdOrcamento &&
                    o.ItemIdItem == dto.ItemIdItem);

            return CreatedAtAction(nameof(Get), new
            {
                orcamentoId = dto.OrcamentoIdOrcamento,
                itemId = dto.ItemIdItem
            }, OrcamentoHasItemMapper.ToDTO(carregado));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao criar associação: {ex.Message}");
        }
    }

    [HttpPut("{orcamentoId}/{itemId}")]
    public async Task<IActionResult> Put(int orcamentoId, int itemId, UpdateOrcamentoHasItemDto dto)
    {
        try
        {
            var entity = await _context.OrcamentoHasItems
                .FirstOrDefaultAsync(o =>
                    o.OrcamentoIdOrcamento == orcamentoId &&
                    o.ItemIdItem == itemId);

            if (entity == null)
                return NotFound("Associação não encontrada.");

            OrcamentoHasItemMapper.ApplyUpdate(dto, entity);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar: {ex.Message}");
        }
    }

    [HttpDelete("{orcamentoId}/{itemId}")]
    public async Task<IActionResult> Delete(int orcamentoId, int itemId)
    {
        try
        {
            var entity = await _context.OrcamentoHasItems
                .FirstOrDefaultAsync(o =>
                    o.OrcamentoIdOrcamento == orcamentoId &&
                    o.ItemIdItem == itemId);

            if (entity == null)
                return NotFound("Associação não encontrada.");

            _context.OrcamentoHasItems.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao remover associação: {ex.Message}");
        }
    }
}
