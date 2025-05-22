using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class OrcamentoHasItem
{
    public long OrcamentoIdOrcamento { get; set; }

    public long OrcamentoUsuarioIdUsuario { get; set; }

    public long ItemIdItem { get; set; }

    public int Quantidade { get; set; }

    public virtual Item ItemIdItemNavigation { get; set; } = null!;

    public virtual Orcamento Orcamento { get; set; } = null!;
}
