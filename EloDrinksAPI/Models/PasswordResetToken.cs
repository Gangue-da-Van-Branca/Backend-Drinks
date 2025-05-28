using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EloDrinksAPI.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public DateTime Expiration { get; set; }

        [ForeignKey("UserId")]
        public Usuario Usuario { get; set; } = null!;
    }
}
