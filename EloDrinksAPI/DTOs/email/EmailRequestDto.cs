using System.ComponentModel.DataAnnotations;

namespace EloDrinksAPI.DTOs.email
{
    public class EmailRequestDto
    {
        [Required]
        public string To { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }
    }
}