using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class Orcamento
{
    public string IdOrcamento { get; set; } = null!;

    public DateOnly Data { get; set; }

    public int Cep { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFim { get; set; }

    public int QtdPessoas { get; set; }

    public float Preco { get; set; }

    public string Status { get; set; } = null!;

    public string TipoEvento { get; set; } = null!;

    public string UsuarioIdUsuario { get; set; } = null!;

    public string? Endereco { get; set; }

    public string? DrinksSelecionados { get; set; }

    public virtual ICollection<OrcamentoHasItem> OrcamentoHasItems { get; set; } = new List<OrcamentoHasItem>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual Usuario UsuarioIdUsuarioNavigation { get; set; } = null!;
}
