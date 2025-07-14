using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.DTOs.orcamento;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Authorization;
using EloDrinksAPI.Services;
using EloDrinksAPI.Const;

namespace EloDrinksAPI.Controllers;

/// <summary>
/// Gerencia a criação e o ciclo de vida dos orçamentos.
/// </summary>
[Route("[controller]")]
[ApiController]
public class OrcamentoController : ControllerBase
{
    private readonly ElodrinkContext _context;

    public OrcamentoController(ElodrinkContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca todos os orçamentos.
    /// </summary>
    /// <returns>Uma lista de todos os orçamentos no sistema.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrcamentoResponseDto>>> GetOrcamentos()
    {
        try
        {
            var orcamentos = await _context.Orcamentos
                .Include(o => o.UsuarioIdUsuarioNavigation)
                .Include(o => o.OrcamentoHasItems)
                    .ThenInclude(ohi => ohi.ItemIdItemNavigation)
                .ToListAsync();

            return orcamentos.Select(OrcamentoMapper.ToDTO).ToList();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar orçamentos: {ex.Message}");
        }
    }

    /// <summary>
    /// Busca um orçamento específico pelo seu ID.
    /// </summary>
    /// <param name="id">O ID do orçamento.</param>
    /// <returns>Os dados detalhados do orçamento.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrcamentoFrontInputDto>> GetOrcamento(string id)
    {
        try
        {
            var orcamento = await _context.Orcamentos
                .Include(o => o.UsuarioIdUsuarioNavigation)
                .Include(o => o.OrcamentoHasItems)
                    .ThenInclude(ohi => ohi.ItemIdItemNavigation)
                .FirstOrDefaultAsync(o => o.IdOrcamento == id);

            if (orcamento == null)
                return NotFound();

            return OrcamentoMapper.ToFrontendDTO(orcamento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar orçamento: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria um novo orçamento a partir das seleções do frontend.
    /// </summary>
    /// <remarks>
    /// Este endpoint recebe um objeto complexo com todas as escolhas do usuário e cria o orçamento e o pedido correspondente.
    /// </remarks>
    /// <param name="dto">Dados completos do orçamento vindos do formulário.</param>
    /// <returns>Os IDs do orçamento e do pedido criados.</returns>
    [HttpPost("front-create")]
    public async Task<IActionResult> CriarOrcamentoViaFrontend([FromBody] OrcamentoFrontInputDto dto)
    {
        try
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.InfosContratante.Email);
            if (usuario == null)
                return BadRequest("Usuário com este e-mail não está cadastrado.");

            var orcamento = new Orcamento
            {
                IdOrcamento = "o1" + GerarIdService.GerarIdAlfanumerico(16),
                Data = DateOnly.Parse(dto.InfosContratante.Data),
                HoraInicio = TimeOnly.Parse(dto.InfosContratante.HorarioInicio),
                HoraFim = TimeOnly.Parse(dto.InfosContratante.HorarioFinal),
                Cep = TratarCep(dto.InfosContratante.Cep),
                QtdPessoas = int.Parse(dto.InfosContratante.Convidados),
                Endereco = dto.InfosContratante.Endereco,
                TipoEvento = dto.BaseFesta.TipoFesta,
                Status = "pendente",
                UsuarioIdUsuario = usuario.IdUsuario,
                DrinksSelecionados = string.Join(", ", dto.BaseFesta.DrinksSelecionados.Select(d => d.Nome))
            };

            _context.Orcamentos.Add(orcamento);

            foreach (var drinkDto in dto.BaseFesta.DrinksSelecionados)
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Nome == drinkDto.Nome);
                if (item != null)
                {
                    _context.OrcamentoHasItems.Add(new OrcamentoHasItem
                    {
                        OrcamentoIdOrcamento = orcamento.IdOrcamento,
                        OrcamentoUsuarioIdUsuario = usuario.IdUsuario,
                        ItemIdItem = item.IdItem,
                        Quantidade = 1
                    });
                }
            }

            async Task AddItens(Dictionary<string, int> itens)
            {
                foreach (var i in itens.Where(i => i.Value > 0))
                {
                    var item = await _context.Items.FirstOrDefaultAsync(x => x.Nome == i.Key);
                    if (item != null)
                    {
                        _context.OrcamentoHasItems.Add(new OrcamentoHasItem
                        {
                            OrcamentoIdOrcamento = orcamento.IdOrcamento,
                            OrcamentoUsuarioIdUsuario = usuario.IdUsuario,
                            ItemIdItem = item.IdItem,
                            Quantidade = i.Value
                        });
                    }
                }
            }

            await AddItens(dto.Opcionais.Shots);
            await AddItens(dto.Opcionais.Extras);

            foreach (var bar in dto.Opcionais.BaresAdicionais)
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Nome == bar);
                if (item != null)
                {
                    _context.OrcamentoHasItems.Add(new OrcamentoHasItem
                    {
                        OrcamentoIdOrcamento = orcamento.IdOrcamento,
                        OrcamentoUsuarioIdUsuario = usuario.IdUsuario,
                        ItemIdItem = item.IdItem,
                        Quantidade = 1
                    });
                }
            }

            await _context.SaveChangesAsync();

            var itensOrcamento = await _context.OrcamentoHasItems
                .Include(i => i.ItemIdItemNavigation)
                .Where(i => i.OrcamentoIdOrcamento == orcamento.IdOrcamento &&
                            i.OrcamentoUsuarioIdUsuario == usuario.IdUsuario)
                .ToListAsync();

            var nomesDrinksBaseFesta = dto.BaseFesta.DrinksSelecionados.Select(d => d.Nome).ToList();

            float totalPedido = itensOrcamento
                .Where(i => !nomesDrinksBaseFesta.Contains(i.ItemIdItemNavigation.Nome))
                .Sum(i => i.ItemIdItemNavigation.Preco * i.Quantidade);

            string tipoFesta = dto.BaseFesta.TipoFesta?.Trim() ?? "";
            float valorTipoFesta = TipoFesta.TipoFestaValores.TryGetValue(tipoFesta, out var valor) ? valor : TipoFesta.ValorOutro;

            int convidados = int.TryParse(dto.InfosContratante.Convidados, out var qtd) ? qtd : 0;
            float valorPorPessoa = convidados * 85f;

            float totalFinal = totalPedido + valorTipoFesta + valorPorPessoa;

            orcamento.Preco = totalFinal;
            _context.Entry(orcamento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var pedido = new Pedido
            {
                IdPedido = "p1" + GerarIdService.GerarIdAlfanumerico(16),
                OrcamentoIdOrcamento = orcamento.IdOrcamento,
                OrcamentoUsuarioIdUsuario = usuario.IdUsuario,
                Total = totalFinal,
                Status = "Pendente",
                DataCriacao = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { idOrcamento = orcamento.IdOrcamento, idPedido = pedido.IdPedido });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Erro ao salvar no banco: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (FormatException ex)
        {
            return BadRequest($"Erro de formatação: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao criar orçamento: {ex.Message}");
        }
    }

    private string TratarCep(string rawCep)
    {
        if (string.IsNullOrWhiteSpace(rawCep))
            throw new ArgumentException("CEP nao pode ser null");

        string somenteNumeros = new string(rawCep.Where(char.IsDigit).ToArray());

        if (somenteNumeros.Length != 8)
            throw new ArgumentException("CEP deve ter 8 digitos");

        return somenteNumeros;
    }

    /// <summary>
    /// Atualiza um orçamento existente.
    /// </summary>
    /// <param name="idOrcamento">ID do orçamento.</param>
    /// <param name="idUsuario">ID do usuário associado ao orçamento.</param>
    /// <param name="dto">Dados a serem atualizados.</param>
    [HttpPut("{idOrcamento}/{idUsuario}")]
    public async Task<IActionResult> PutOrcamento(string idOrcamento, string idUsuario, UpdateOrcamentoDto dto)
    {
        try
        {
            var orcamento = await _context.Orcamentos.FindAsync(idOrcamento, idUsuario);
            if (orcamento == null)
                return NotFound("Orçamento não encontrado.");

            OrcamentoMapper.ApplyUpdate(dto, orcamento);
            _context.Entry(orcamento).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok("Orçamento atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar orçamento: {ex.Message}");
        }
    }

    /// <summary>
    /// Deleta um orçamento.
    /// </summary>
    /// <param name="idOrcamento">ID do orçamento a ser deletado.</param>
    /// <param name="idUsuario">ID do usuário associado ao orçamento.</param>
    [HttpDelete("{idOrcamento}/{idUsuario}")]
    public async Task<IActionResult> DeleteOrcamento(string idOrcamento, string idUsuario)
    {
        try
        {
            var orcamento = await _context.Orcamentos.FindAsync(idOrcamento, idUsuario);
            if (orcamento == null)
                return NotFound("Orçamento não encontrado.");

            _context.Orcamentos.Remove(orcamento);
            await _context.SaveChangesAsync();

            return Ok("Orçamento deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao deletar orçamento: {ex.Message}");
        }
    }
}