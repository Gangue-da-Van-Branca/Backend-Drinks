using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.email
{
    public class EmailRequestDto
    {
        [Required]
        public string To { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;
    }
}