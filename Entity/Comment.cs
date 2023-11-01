using System;
using System.Collections.Generic;

namespace SHOU.Entity;

public partial class Comment
{
    public string Id { get; set; } = null!;

    public string IdUser { get; set; } = null!;

    public string IdPost { get; set; } = null!;

    public string? Comment1 { get; set; }

    public string? IdParent { get; set; }

    public DateTime? AtTime { get; set; }
}
