using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Models;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.DTOs.orcamentoHasItem;
using Microsoft.AspNetCore.Authorization;

namespace EloDrinksAPI.Controllers;

/// <summary>
/// Gerencia a relação entre orçamentos e os itens que eles contêm.
/// </summary>
[Route("[controller]")]
[ApiController]
public class OrcamentoHasItemController : ControllerBase
{
    private readonly ElodrinkContext _context;

    public OrcamentoHasItemController(ElodrinkContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca todas as associações de itens e orçamentos.
    /// </summary>
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

    /// <summary>
    /// Busca uma associação específica entre um orçamento e um item.
    /// </summary>
    /// <param name="orcamentoId">ID do orçamento.</param>
    /// <param name="itemId">ID do item.</param>
    [HttpGet("{orcamentoId}/{itemId}")]
    public async Task<ActionResult<OrcamentoHasItemResponseDto>> Get(string orcamentoId, string itemId)
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

    /// <summary>
    /// Busca todos os itens de um orçamento específico.
    /// </summary>
    /// <param name="orcamentoId">ID do orçamento.</param>
    [HttpGet("Orcamento/{orcamentoId}")]
    public async Task<ActionResult<IEnumerable<OrcamentoHasItemResponseDto>>> GetByOrcamento(string orcamentoId)
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

    /// <summary>
    /// Associa um novo item a um orçamento.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrcamentoHasItemResponseDto>> Post(CreateOrcamentoHasItemDto dto)
    {
        try
        {
            var orcamentoExiste = await _context.Orcamentos
                .AnyAsync(o => o.IdOrcamento == dto.OrcamentoIdOrcamento);
            var itemExiste = await _context.Items
                .AnyAsync(i => i.IdItem == dto.ItemIdItem);

            if (!orcamentoExiste)
                return NotFound("Orçamento não encontrado.");

            if (!itemExiste)
                return NotFound("Item não encontrado.");

            var existe = await _context.OrcamentoHasItems.AnyAsync(e =>
                e.OrcamentoIdOrcamento == dto.OrcamentoIdOrcamento &&
                e.ItemIdItem == dto.ItemIdItem);

            if (existe)
                return Conflict("Item já está associado a este orçamento.");

            var entity = OrcamentoHasItemMapper.ToEntity(dto);
            _context.OrcamentoHasItems.Add(entity);
            await _context.SaveChangesAsync();

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

    /// <summary>
    /// Atualiza uma associação (ex: quantidade) entre item e orçamento.
    /// </summary>
    [HttpPut("{orcamentoId}/{itemId}")]
    public async Task<IActionResult> Put(string orcamentoId, string itemId, UpdateOrcamentoHasItemDto dto)
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

            return Ok("Associação atualizada com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove um item de um orçamento.
    /// </summary>
    [HttpDelete("{orcamentoId}/{itemId}")]
    public async Task<IActionResult> Delete(string orcamentoId, string itemId)
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

            return Ok("Associação deletada com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao remover associação: {ex.Message}");
        }
    }
}