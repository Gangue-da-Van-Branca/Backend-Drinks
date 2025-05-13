using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Models;

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
    public async Task<ActionResult<IEnumerable<OrcamentoHasItem>>> GetAll()
    {
        return await _context.OrcamentoHasItems
            .Include(ohi => ohi.ItemIdItemNavigation)
            .Include(ohi => ohi.Orcamento)
            .ToListAsync();
    }

    [HttpGet("{orcamentoId}/{itemId}")]
    public async Task<ActionResult<OrcamentoHasItem>> Get(int orcamentoId, int itemId)
    {
        var ohi = await _context.OrcamentoHasItems
            .Include(ohi => ohi.ItemIdItemNavigation)
            .Include(ohi => ohi.Orcamento)
            .FirstOrDefaultAsync(o => o.OrcamentoIdOrcamento == orcamentoId && o.ItemIdItem == itemId);

        if (ohi == null)
            return NotFound();

        return ohi;
    }

    [HttpGet("Orcamento/{orcamentoId}")]
    public async Task<ActionResult<IEnumerable<OrcamentoHasItem>>> GetByOrcamento(int orcamentoId)
    {
        var itens = await _context.OrcamentoHasItems
            .Where(o => o.OrcamentoIdOrcamento == orcamentoId)
            .Include(ohi => ohi.ItemIdItemNavigation)
            .ToListAsync();

        return itens;
    }

    [HttpPost]
    public async Task<ActionResult<OrcamentoHasItem>> Post(OrcamentoHasItem entity)
    {
        var orcamentoExiste = await _context.Orcamentos.AnyAsync(o => o.IdOrcamento == entity.OrcamentoIdOrcamento);
        var itemExiste = await _context.Items.AnyAsync(i => i.IdItem == entity.ItemIdItem);

        if (!orcamentoExiste || !itemExiste)
            return BadRequest("Orçamento ou Item não encontrado.");

        var existe = await _context.OrcamentoHasItems.AnyAsync(e =>
            e.OrcamentoIdOrcamento == entity.OrcamentoIdOrcamento &&
            e.ItemIdItem == entity.ItemIdItem);

        if (existe)
            return Conflict("Item já está associado a este orçamento.");

        _context.OrcamentoHasItems.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { orcamentoId = entity.OrcamentoIdOrcamento, itemId = entity.ItemIdItem }, entity);
    }

    [HttpPut("{orcamentoId}/{itemId}")]
    public async Task<IActionResult> Put(int orcamentoId, int itemId, OrcamentoHasItem entity)
    {
        if (orcamentoId != entity.OrcamentoIdOrcamento || itemId != entity.ItemIdItem)
            return BadRequest("Chaves primárias não coincidem.");

        _context.Entry(entity).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var existe = await _context.OrcamentoHasItems.AnyAsync(e =>
                e.OrcamentoIdOrcamento == orcamentoId &&
                e.ItemIdItem == itemId);

            if (!existe)
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{orcamentoId}/{itemId}")]
    public async Task<IActionResult> Delete(int orcamentoId, int itemId)
    {
        var entity = await _context.OrcamentoHasItems
            .FirstOrDefaultAsync(e =>
                e.OrcamentoIdOrcamento == orcamentoId &&
                e.ItemIdItem == itemId);

        if (entity == null)
            return NotFound();

        _context.OrcamentoHasItems.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
