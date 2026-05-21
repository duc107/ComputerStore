using System;
using System.Collections.Generic;

namespace ComputerStore.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int? StockQuantity { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
