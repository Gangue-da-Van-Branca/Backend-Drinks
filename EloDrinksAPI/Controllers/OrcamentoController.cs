using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EloDrinksAPI.DTOs.orcamento;
using EloDrinksAPI.Mappers;
using EloDrinksAPI.Models;
using Microsoft.AspNetCore.Authorization;
using EloDrinksAPI.Services;
using EloDrinksAPI.Const;

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

    //[Authorize(Roles = "admin")]
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

    //[Authorize(Roles = "admin")]
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

    [HttpPost("front-create")]
    public async Task<IActionResult> CriarOrcamentoViaFrontend([FromBody] OrcamentoFrontInputDto dto)
    {
        try
        {
            // Verifica se o usuário já existe
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.InfosContratante.Email);
            if (usuario == null)
                return BadRequest("Usuário com este e-mail não está cadastrado.");

            // Cria orçamento
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

            // Adiciona drinks
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

            // Adiciona opcionais
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
                    else
                    {
                        Console.WriteLine($"AVISO: Item opcional '{i.Key}' não encontrado.");
                    }
                }
            }

            await AddItens(dto.Opcionais.Shots);
            await AddItens(dto.Opcionais.Extras);

            // Bares adicionais
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
                else
                {
                    Console.WriteLine($"AVISO: Bar adicional '{bar}' não encontrado.");
                }
            }

            await _context.SaveChangesAsync();

            // calculo do total
            var itensOrcamento = await _context.OrcamentoHasItems
                .Include(i => i.ItemIdItemNavigation)
                .Where(i => i.OrcamentoIdOrcamento == orcamento.IdOrcamento &&
                            i.OrcamentoUsuarioIdUsuario == usuario.IdUsuario)
                .ToListAsync();

            var nomesDrinksBaseFesta = dto.BaseFesta.DrinksSelecionados.Select(d => d.Nome).ToList(); // lista do q vai er ignorado na conta

            float totalPedido = itensOrcamento
                .Where(i => !nomesDrinksBaseFesta.Contains(i.ItemIdItemNavigation.Nome))
                .Sum(i => i.ItemIdItemNavigation.Preco * i.Quantidade);

            // adicional por tipo de festa
            string tipoFesta = dto.BaseFesta.TipoFesta?.Trim() ?? "";
            // se achar usa o valor, se n achar é pq é valor de festa custom
            float valorTipoFesta = TipoFesta.TipoFestaValores.TryGetValue(tipoFesta, out var valor) ? valor: TipoFesta.ValorOutro;

            // adicional por convidados (85/pessoa)
            int convidados = int.TryParse(dto.InfosContratante.Convidados, out var qtd) ? qtd : 0;
            float valorPorPessoa = convidados * 85f;

            // valor final
            float totalFinal = totalPedido + valorTipoFesta + valorPorPessoa;

            // att orçamento cm valor final
            orcamento.Preco = totalFinal;
            _context.Entry(orcamento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Cria o pedido automaticamente
            var pedido = new Pedido
            {
                IdPedido = "p1" + GerarIdService.GerarIdAlfanumerico(16),
                OrcamentoIdOrcamento = orcamento.IdOrcamento,
                OrcamentoUsuarioIdUsuario = usuario.IdUsuario,
                Total = totalFinal, // valor com as logicas de tipoFesta e n convidados
                Status = "Pendente",
                DataCriacao = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { idOrcamento = orcamento.IdOrcamento, idPedido = pedido.IdPedido });
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, $"Erro ao salvar no banco: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"FormatException: {ex.Message}");
            return BadRequest($"Erro de formatação: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, $"Erro ao criar orçamento: {ex.Message}");
        }
    }

    // O método TratarCep permanece o mesmo
    private string TratarCep(string rawCep)
    {
        if (string.IsNullOrWhiteSpace(rawCep))
            throw new ArgumentException("CEP nao pode ser null");

        string somenteNumeros = new string(rawCep.Where(char.IsDigit).ToArray());

        if (somenteNumeros.Length != 8)
            throw new ArgumentException("CEP deve ter 8 digitos");

        return somenteNumeros;
    }

    // PUT: api/Orcamento/5
    //[Authorize(Roles = "admin")]
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
    //[Authorize(Roles = "admin")]
    [HttpDelete("{idOrcamento}/{idUsuario}")]
    public async Task<IActionResult> DeleteOrcamento(string idOrcamento, string idUsuario)
    {
        try
        {
            var orcamento = await _context.Orcamentos.FindAsync(idOrcamento, idUsuario);
            if (orcamento == null)
                return NotFound();

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
