using System;
using System.Collections.Generic;

namespace ComputerStore.Models;

public partial class Appuser
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Chatmessage> Chatmessages { get; set; } = new List<Chatmessage>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Warrantyticket> Warrantytickets { get; set; } = new List<Warrantyticket>();
}
