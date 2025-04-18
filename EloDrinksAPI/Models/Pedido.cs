using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public float Total { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly DataCriacao { get; set; }

    public int OrcamentoIdOrcamento { get; set; }

    public int OrcamentoUsuarioIdUsuario { get; set; }

    public virtual Orcamento Orcamento { get; set; } = null!;
}
