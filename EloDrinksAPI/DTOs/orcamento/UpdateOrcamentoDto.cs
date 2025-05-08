using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.DTOs.orcamento
{
    public class UpdateOrcamentoDto
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
    }
}