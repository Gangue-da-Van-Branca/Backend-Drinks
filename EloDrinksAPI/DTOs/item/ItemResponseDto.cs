using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.DTOs.item
{
    public class ItemResponseDto
    {
        public int IdItem { get; set; }
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public float Preco { get; set; }
        public string Tipo { get; set; } = null!;
    }
}