using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.orcamento
{
    public class CreateOrcamentoDto
    {
        public string IdOrcamento { get; set; } = null!;

        [Required]
        public DateOnly Data { get; set; }

        [Required]
        public string Cep { get; set; } = null!;

        [Required]
        public TimeOnly HoraInicio { get; set; }

        [Required]
        public TimeOnly HoraFim { get; set; }

        [Range(1, int.MaxValue)]
        public int QtdPessoas { get; set; }

        [Range(0.01, double.MaxValue)]
        public float Preco { get; set; }

        [Required]
        public string Status { get; set; } = null!;

        [Required]
        public string TipoEvento { get; set; } = null!;

        [Required]
        public string UsuarioIdUsuario { get; set; } = null!;

        [Required]
        public string Endereco { get; set; } = null!;

        public string? DrinksSelecionados { get; set; } // opcional, usado para armazenar a lista como string

        public List<OrcamentoItemDto>? Itens { get; set; } // drinks + opcionais

    }

    public class OrcamentoItemDto
    {
        public string IdItem { get; set; } = null!;
        public int Quantidade { get; set; }
    }

}