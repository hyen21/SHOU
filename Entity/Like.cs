using System;
using System.Collections.Generic;

namespace SHOU.Entity;

public partial class Like
{
    public string Id { get; set; } = null!;

    public string IdUser { get; set; } = null!;

    public string IdPost { get; set; } = null!;
}
