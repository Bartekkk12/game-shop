using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GameShop.Models;

public class Order
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public DateTime OrderDate { get; set; }

    [DefaultValue("New")]
    public Status status { get; set; }

    // Navigation
    public User? User { get; set; }
    
    // 1 → N
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public enum Status
    {
        Cart,
        New,
        PaymentReceived,
        PaymentSuceeded,
        PaymentRejected,
        InProgress,
        Sent,
    }
}