using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.DTOs.item
{
    public class CreateItemDto
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Descricao { get; set; } = null!;

        [Range(0.01, 9999.99)]
        public float Preco { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = null!;
    }
}