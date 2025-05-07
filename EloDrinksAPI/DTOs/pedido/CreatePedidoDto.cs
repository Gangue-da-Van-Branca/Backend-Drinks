using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.pedido
{
    public class CreatePedidoDto
    {
        [Required]
        public float Total { get; set; }

        [Required]
        [RegularExpression(@"^(Pendente|Pago|Cancelado)$", ErrorMessage = "Status inválido.")]
        public string Status { get; set; } = null!;

        [Required]
        public int OrcamentoIdOrcamento { get; set; }

        [Required]
        public int OrcamentoUsuarioIdUsuario { get; set; }
    }

}