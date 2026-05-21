using System;
using System.Collections.Generic;

namespace ComputerStore.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? UserId { get; set; }

    public int? PromotionId { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual Promotion? Promotion { get; set; }

    public virtual Appuser? User { get; set; }
}
