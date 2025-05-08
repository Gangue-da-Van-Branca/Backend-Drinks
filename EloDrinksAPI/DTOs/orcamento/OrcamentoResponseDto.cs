using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.DTOs.orcamento
{
    public class OrcamentoResponseDto
    {
        public int IdOrcamento { get; set; }

        public DateOnly Data { get; set; }

        public int Cep { get; set; }

        public TimeOnly HoraInicio { get; set; }

        public TimeOnly HoraFim { get; set; }

        public int QtdPessoas { get; set; }

        public float Preco { get; set; }

        public string Status { get; set; } = null!;

        public string TipoEvento { get; set; } = null!;

        public int UsuarioIdUsuario { get; set; }

        public string? NomeUsuario { get; set; } // opcional
    }
}