using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.orcamento
{
    public class CreateOrcamentoDto
    {
        [Required]
        public DateOnly Data { get; set; }

        [Required]
        public int Cep { get; set; }

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
        public string UsuarioIdUsuario { get; set; }
    }
}