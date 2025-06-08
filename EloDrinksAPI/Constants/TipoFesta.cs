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
            { "Casamento", 10000 },
            { "Evento Corporativo", 7000 },
            { "Evento de Lançamento", 5000 },
            { "Coquetel", 4000 },
            { "Aniversário", 5000 },
            { "Debutante", 8000 },
            { "Festa Teen", 3000 }
        };

        public const float ValorOutro = 8000;
    }
}