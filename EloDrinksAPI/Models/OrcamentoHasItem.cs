using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class OrcamentoHasItem
{
    public int OrcamentoIdOrcamento { get; set; }

    public int OrcamentoUsuarioIdUsuario { get; set; }

    public int ItemIdItem { get; set; }

    public int Quantidade { get; set; }

    public virtual Item ItemIdItemNavigation { get; set; } = null!;

    public virtual Orcamento Orcamento { get; set; } = null!;
}
