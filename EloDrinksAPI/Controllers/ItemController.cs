using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EloDrinksAPI.DTOs.item;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.Services;

namespace EloDrinksAPI.Controllers;

/// <summary>
/// Gerencia o catálogo de itens (drinks, shots, bares, opcionais, etc.).
/// </summary>
[Route("[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly ElodrinkContext _context;

    public ItemController(ElodrinkContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca a lista completa de itens disponíveis.
    /// </summary>
    /// <returns>Uma coleção de todos os itens.</returns>
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

    /// <summary>
    /// Busca um item específico pelo seu ID.
    /// </summary>
    /// <param name="id">O ID único do item.</param>
    /// <returns>Os dados do item solicitado.</returns>
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

    /// <summary>
    /// Cria um ou mais itens em lote.
    /// </summary>
    /// <param name="dtos">Uma lista de objetos com os dados dos itens a serem criados.</param>
    [HttpPost()]
    public async Task<ActionResult<IEnumerable<ItemResponseDto>>> PostItensEmLote(List<CreateItemDto> dtos)
    {
        try
        {
            var itensCriados = new List<Item>();

            foreach (var dto in dtos)
            {
                if (_context.Items.Any(i => i.Nome == dto.Nome))
                {
                    return BadRequest($"Item com nome '{dto.Nome}' já existe.");
                }

                var entity = ItemMapper.ToEntity(dto);
                entity.IdItem = "i1" + GerarIdService.GerarIdAlfanumerico(16);
                itensCriados.Add(entity);
            }

            _context.Items.AddRange(itensCriados);
            await _context.SaveChangesAsync();

            var result = itensCriados.Select(ItemMapper.ToDTO).ToList();
            return Ok("Item cadastrado com sucesso.");
        }
        catch
        {
            return StatusCode(500, "Erro ao criar os itens em lote.");
        }
    }


    /// <summary>
    /// Atualiza os dados de um item existente.
    /// </summary>
    /// <param name="id">O ID do item a ser atualizado.</param>
    /// <param name="dto">Os novos dados para o item.</param>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutItem(string id, UpdateItemDto dto)
    {
        try
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound("Item não encontrado.");

            ItemMapper.ApplyUpdate(dto, item);
            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok("Item atualizado com sucesso.");
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

    /// <summary>
    /// Exclui um item do sistema.
    /// </summary>
    /// <param name="id">O ID do item a ser excluído.</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(string id)
    {
        try
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound("Item não encontrado.");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Item deletado com sucesso.");
        }
        catch
        {
            return StatusCode(500, "Erro ao excluir o item.");
        }
    }

    private bool ItemExists(string id) => _context.Items.Any(e => e.IdItem == id);
}