using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.usuario
{
    public class UpdateUsuarioDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MinLength(2), MaxLength(50)]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O sobrenome é obrigatório.")]
        [MinLength(2), MaxLength(50)]
        public string Sobrenome { get; set; } = null!;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone]
        public string Telefone { get; set; } = null!;

        // [Required(ErrorMessage = "A senha é obrigatória.")]
        // [MinLength(6)]
        // public string Senha { get; set; } = null!;

        [Required(ErrorMessage = "O tipo é obrigatório.")]
        public string Tipo { get; set; } = null!;
    }
}