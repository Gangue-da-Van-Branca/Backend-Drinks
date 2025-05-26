using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.pedido
{
    public class CreatePedidoDto
    {
        [Required]
        public float Total { get; set; }

        [Required]
        public string Status { get; set; } = null!;

        [Required]
        public string OrcamentoIdOrcamento { get; set; } = null!;

        [Required]
        public string OrcamentoUsuarioIdUsuario { get; set; } = null!;
    }

}