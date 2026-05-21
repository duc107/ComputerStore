using System;
using System.Collections.Generic;

namespace ComputerStore.Models;

public partial class Warrantyticket
{
    public int TicketId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string IssueDescription { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public int? CustomerId { get; set; }

    public virtual Appuser? Customer { get; set; }
}
