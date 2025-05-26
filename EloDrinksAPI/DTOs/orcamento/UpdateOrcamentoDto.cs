using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.orcamento
{
    public class UpdateOrcamentoDto
    {
        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateOnly Data { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string Cep { get; set; } = null!;

        [Required(ErrorMessage = "O horário de início é obrigatório.")]
        public TimeOnly HoraInicio { get; set; }

        [Required(ErrorMessage = "O horário de término é obrigatório.")]
        public TimeOnly HoraFim { get; set; }

        [Required(ErrorMessage = "A quantidade de pessoas é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade de pessoas deve ser maior que zero.")]
        public int QtdPessoas { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser negativo.")]
        public float Preco { get; set; }

        [Required(ErrorMessage = "O status é obrigatório.")]
        public string Status { get; set; } = null!;

        [Required(ErrorMessage = "O tipo de evento é obrigatório.")]
        public string TipoEvento { get; set; } = null!;
    }
}
