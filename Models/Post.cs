using System;
using System.Collections.Generic;

namespace SHOU.Models;

public partial class Post
{
    public string Id { get; set; } = null!;

    public string IdUser { get; set; } = null!;

    public string? Image { get; set; }

    public string? Content { get; set; }

    public string? Video { get; set; }

    public DateTime? CreateTime { get; set; }
}
