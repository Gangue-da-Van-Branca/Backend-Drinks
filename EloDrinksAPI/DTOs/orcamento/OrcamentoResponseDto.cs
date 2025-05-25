using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.DTOs.orcamento
{
    public class OrcamentoResponseDto : OrcamentoFrontInputDto
    {
        public string IdOrcamento { get; set; } = null!;
        
        public string IdUsuario { get; set; } = null!;

    }
}