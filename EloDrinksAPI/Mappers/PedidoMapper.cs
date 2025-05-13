using EloDrinksAPI.DTOs.pedido;
using EloDrinksAPI.Models;

public static class PedidoMapper
{
    public static Pedido ToEntity(CreatePedidoDto dto)
    {
        return new Pedido
        {
            Total = dto.Total,
            Status = dto.Status,
            OrcamentoIdOrcamento = dto.OrcamentoIdOrcamento,
            OrcamentoUsuarioIdUsuario = dto.OrcamentoUsuarioIdUsuario,
            DataCriacao = DateOnly.FromDateTime(DateTime.Today)
        };
    }

    public static PedidoResponseDto ToDTO(Pedido pedido)
    {
        return new PedidoResponseDto
        {
            IdPedido = pedido.IdPedido,
            Total = pedido.Total,
            Status = pedido.Status,
            DataCriacao = pedido.DataCriacao,
            OrcamentoIdOrcamento = pedido.OrcamentoIdOrcamento,
            OrcamentoUsuarioIdUsuario = pedido.OrcamentoUsuarioIdUsuario
        };
    }

    public static void ApplyUpdate(UpdatePedidoDto dto, Pedido pedido)
    {
        if (dto.Total != null) pedido.Total = dto.Total.Value;
        if (!string.IsNullOrEmpty(dto.Status)) pedido.Status = dto.Status;
        if (dto.OrcamentoIdOrcamento != null) pedido.OrcamentoIdOrcamento = dto.OrcamentoIdOrcamento.Value;
        if (dto.OrcamentoUsuarioIdUsuario != null) pedido.OrcamentoUsuarioIdUsuario = dto.OrcamentoUsuarioIdUsuario.Value;
    }
}
