using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EloDrinksAPI.DTOs.item;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EloDrinksAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly ElodrinkContext _context;

    public ItemController(ElodrinkContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetItems()
    {
        try
        {
            var items = await _context.Items.ToListAsync();
            return Ok(items.Select(ItemMapper.ToDTO));
        }
        catch
        {
            return StatusCode(500, "Erro ao buscar os itens.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemResponseDto>> GetItem(int id)
    {
        try
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            return Ok(ItemMapper.ToDTO(item));
        }
        catch
        {
            return StatusCode(500, "Erro ao buscar o item.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ItemResponseDto>> PostItem(CreateItemDto dto)
    {
        try
        {
            var entity = ItemMapper.ToEntity(dto);
            _context.Items.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = entity.IdItem }, ItemMapper.ToDTO(entity));
        }
        catch
        {
            return StatusCode(500, "Erro ao criar o item.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutItem(int id, UpdateItemDto dto)
    {
        try
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            ItemMapper.ApplyUpdate(dto, item);
            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ItemExists(id))
                return NotFound();

            return StatusCode(500, "Erro de concorrência ao atualizar o item.");
        }
        catch
        {
            return StatusCode(500, "Erro ao atualizar o item.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        try
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "Erro ao excluir o item.");
        }
    }

    private bool ItemExists(int id) => _context.Items.Any(e => e.IdItem == id);
}
