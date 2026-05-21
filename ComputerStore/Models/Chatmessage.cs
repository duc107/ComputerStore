using System;
using System.Collections.Generic;

namespace ComputerStore.Models;

public partial class Chatmessage
{
    public int MessageId { get; set; }

    public string Content { get; set; } = null!;

    public int? SenderId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Appuser? Sender { get; set; }
}
