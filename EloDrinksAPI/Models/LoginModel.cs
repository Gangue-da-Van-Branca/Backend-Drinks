using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloDrinksAPI.Models
{
    public class LoginModel
    {
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }

}