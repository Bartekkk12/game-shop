using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    [DefaultValue("New")]
    public Status status { get; set; }

    // 1 → N
    public ICollection<OrderItem> OrderItems { get; set; }

    public enum Status
    {
        New,
        PaymentReceived,
        PaymentSuceeded,
        PaymentRejected,
        InProgress,
        Sent,
    }
}