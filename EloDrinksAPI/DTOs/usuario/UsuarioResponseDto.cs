namespace EloDrinksAPI.DTOs.usuario
{
    public class UsuarioResponseDto
    {
        public string IdUsuario { get; set; } = null!;
        
        public string Nome { get; set; } = null!;

        public string Sobrenome { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Telefone { get; set; } = null!;

        public DateOnly DataCadastro { get; set; }

        public string Tipo { get; set; } = null!;
    }
}