using System;
using System.Collections.Generic;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; }

    // 1 → N
    public ICollection<OrderItem> OrderItems { get; set; }
}
