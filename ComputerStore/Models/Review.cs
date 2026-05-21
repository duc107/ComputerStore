using System;
using System.Collections.Generic;

namespace ComputerStore.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Appuser? User { get; set; }
}
