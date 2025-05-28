namespace EloDrinksAPI.DTOs.forgotPassword
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = null!;
        public string NovaSenha { get; set; } = null!;
    }
}