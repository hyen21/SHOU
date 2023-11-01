using System;
using System.Collections.Generic;

namespace SHOU.Entity;

public partial class Image
{
    public string Id { get; set; } = null!;

    public string IdUser { get; set; } = null!;

    public string? ImageUrl { get; set; }
}
