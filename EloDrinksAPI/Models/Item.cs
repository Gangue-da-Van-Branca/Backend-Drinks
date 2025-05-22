using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class Item
{
    public long IdItem { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public float Preco { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<OrcamentoHasItem> OrcamentoHasItems { get; set; } = new List<OrcamentoHasItem>();
}
