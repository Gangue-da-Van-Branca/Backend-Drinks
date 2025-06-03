using System;
using System.Collections.Generic;

namespace EloDrinksAPI.Models;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime Expiration { get; set; }

    public virtual Usuario User { get; set; } = null!;
}
