using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.Const
{
    public class TipoFesta
    {
        // dicionario para os valores das festas
        public static readonly Dictionary<string, float> TipoFestaValores = new()
        {
            { "Casamento", 100 },
            { "Evento Corporativo", 200 },
            { "Evento de Lançamento", 300 },
            { "Coquetel", 400 },
            { "Aniversário", 500 },
            { "Debutante", 600 },
            { "Festa Teen", 700 }
        };

        public const float ValorOutro = 800; // qqr input do cliente
    }
}