using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.pedido
{
    public class UpdatePedidoDto
    {
        public float? Total { get; set; }

        [RegularExpression(@"^(Pendente|Pago|Cancelado)$", ErrorMessage = "Status inválido.")]
        public string? Status { get; set; }

        public string? OrcamentoIdOrcamento { get; set; }

        public string? OrcamentoUsuarioIdUsuario { get; set; }
    }

}