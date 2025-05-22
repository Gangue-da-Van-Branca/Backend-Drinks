using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class OrcamentoHasItem
{
    public string OrcamentoIdOrcamento { get; set; } = null!;

    public string OrcamentoUsuarioIdUsuario { get; set; } = null!;

    public string ItemIdItem { get; set; } = null!;

    public int Quantidade { get; set; }

    public virtual Item ItemIdItemNavigation { get; set; } = null!;

    public virtual Orcamento Orcamento { get; set; } = null!;
}
