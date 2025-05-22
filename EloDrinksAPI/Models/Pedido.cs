using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class Pedido
{
    public long IdPedido { get; set; }

    public float Total { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly DataCriacao { get; set; }

    public long OrcamentoIdOrcamento { get; set; }

    public long OrcamentoUsuarioIdUsuario { get; set; }

    public virtual Orcamento Orcamento { get; set; } = null!;
}
