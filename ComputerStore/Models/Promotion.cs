using System;
using System.Collections.Generic;

namespace ComputerStore.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public DateTime ExpiryDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
