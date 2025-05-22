namespace EloDrinksAPI.DTOs.pedido
{
    public class PedidoResponseDto
    {
        public string IdPedido { get; set; } = null!;

        public float Total { get; set; }

        public string Status { get; set; } = null!;

        public DateOnly DataCriacao { get; set; }

        public string OrcamentoIdOrcamento { get; set; } = null!;

        public string OrcamentoUsuarioIdUsuario { get; set; } = null!;
    }

}