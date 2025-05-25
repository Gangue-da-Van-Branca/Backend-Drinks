using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.DTOs.orcamento
{
    public class OrcamentoFrontInputDto
    {
        public BaseFestaDto BaseFesta { get; set; } = null!;
        public InfosContratanteDto InfosContratante { get; set; } = null!;
        public OpcionaisDto Opcionais { get; set; } = null!;
    }

    public class BaseFestaDto
    {
        public string TipoFesta { get; set; } = null!;
        public List<DrinkDto> DrinksSelecionados { get; set; } = new();
    }

    public class DrinkDto
    {
        public string Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
    }

    public class InfosContratanteDto
    {
        public string Nome { get; set; } = null!;
        public string Sobrenome { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Data { get; set; } = null!;
        public string Endereco { get; set; } = null!;
        public string HorarioInicio { get; set; } = null!;
        public string HorarioFinal { get; set; } = null!;
        public string Cep { get; set; } = null!;
        public string Convidados { get; set; } = null!;
    }

    public class OpcionaisDto
    {
        public Dictionary<string, int> Shots { get; set; } = new();
        public Dictionary<string, int> Extras { get; set; } = new();
        public List<string> BaresAdicionais { get; set; } = new();
    }
}