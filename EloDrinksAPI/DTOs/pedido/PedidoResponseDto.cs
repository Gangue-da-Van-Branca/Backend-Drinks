namespace EloDrinksAPI.DTOs.pedido
{
    public class PedidoResponseDto
    {
        public long IdPedido { get; set; }

        public float Total { get; set; }

        public string Status { get; set; } = null!;

        public DateOnly DataCriacao { get; set; }

        public long OrcamentoIdOrcamento { get; set; }

        public long OrcamentoUsuarioIdUsuario { get; set; }
    }

}