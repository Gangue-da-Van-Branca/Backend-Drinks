using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public DateTime Created { get; set; }

        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}