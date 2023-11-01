using System;
using System.Collections.Generic;

namespace SHOU.Entity;

public partial class User
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Avatar { get; set; }

    public DateTime? Birthday { get; set; }

    public bool? Gender { get; set; }
}
