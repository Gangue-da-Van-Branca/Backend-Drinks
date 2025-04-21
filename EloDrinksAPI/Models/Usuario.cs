using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nome { get; set; } = null!;

    public string Sobrenome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public DateOnly DataCadastro { get; set; }

    public string Senha { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}
