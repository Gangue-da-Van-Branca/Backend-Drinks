using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class Pedido
{
    public string IdPedido { get; set; } = null!;

    public float Total { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly DataCriacao { get; set; }

    public string OrcamentoIdOrcamento { get; set; } = null!;

    public string OrcamentoUsuarioIdUsuario { get; set; } = null!;

    public virtual Orcamento Orcamento { get; set; } = null!;
}
