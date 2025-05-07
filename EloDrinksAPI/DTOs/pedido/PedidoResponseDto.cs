namespace EloDrinksAPI.DTOs.pedido
{
    public class PedidoResponseDto
    {
        public int IdPedido { get; set; }

        public float Total { get; set; }

        public string Status { get; set; } = null!;

        public DateOnly DataCriacao { get; set; }

        public int OrcamentoIdOrcamento { get; set; }

        public int OrcamentoUsuarioIdUsuario { get; set; }
    }

}