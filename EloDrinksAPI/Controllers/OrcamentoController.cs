using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Authorization;

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
    public async Task<ActionResult<IEnumerable<Orcamento>>> GetOrcamentos()
    {
        return await _context.Orcamentos
            .Include(o => o.UsuarioIdUsuarioNavigation)
            .Include(o => o.OrcamentoHasItems)
            .Include(o => o.Pedidos)
            .ToListAsync();
    }

    // GET: api/Orcamento/5
    [Authorize(Roles = "admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Orcamento>> GetOrcamento(int id)
    {
        var orcamento = await _context.Orcamentos
            .Include(o => o.UsuarioIdUsuarioNavigation)
            .Include(o => o.OrcamentoHasItems)
            .Include(o => o.Pedidos)
            .FirstOrDefaultAsync(o => o.IdOrcamento == id);

        if (orcamento == null)
        {
            return NotFound();
        }

        return orcamento;
    }

    // POST: api/Orcamento
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<ActionResult<Orcamento>> PostOrcamento(Orcamento orcamento)
    {
        _context.Orcamentos.Add(orcamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrcamento), new { id = orcamento.IdOrcamento }, orcamento);
    }

    // PUT: api/Orcamento/5
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrcamento(int id, Orcamento orcamento)
    {
        if (id != orcamento.IdOrcamento)
        {
            return BadRequest();
        }

        _context.Entry(orcamento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrcamentoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Orcamento/5
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrcamento(int id)
    {
        var orcamento = await _context.Orcamentos.FindAsync(id);
        if (orcamento == null)
        {
            return NotFound();
        }

        _context.Orcamentos.Remove(orcamento);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrcamentoExists(int id)
    {
        return _context.Orcamentos.Any(e => e.IdOrcamento == id);
    }
}
