using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EloDrinksAPI.DTOs.item;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Services;

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
    public async Task<ActionResult<ItemResponseDto>> GetItem(string id)
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

    [HttpPost()]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> PostItensEmLote(List<CreateItemDto> dtos)
    {
        try
        {
            var itensCriados = new List<Item>();

            foreach (var dto in dtos)
            {
                var entity = ItemMapper.ToEntity(dto);
                entity.IdItem = "i1" + GerarIdService.GerarIdAlfanumerico(16);
                itensCriados.Add(entity);
            }

            _context.Items.AddRange(itensCriados);
            await _context.SaveChangesAsync();

            var result = itensCriados.Select(ItemMapper.ToDTO).ToList();
            return Ok(result);
        }
        catch
        {
            return StatusCode(500, "Erro ao criar os itens em lote.");
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> PutItem(string id, UpdateItemDto dto)
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
    public async Task<IActionResult> DeleteItem(string id)
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

    private bool ItemExists(string id) => _context.Items.Any(e => e.IdItem == id);
}
