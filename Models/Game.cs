using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Game
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public DateTime ReleaseDate { get; set; }

    public int Stock { get; set; }

    // FK
    public int CategoryId { get; set; }
    public int PublisherId { get; set; }

    // Navigation
    public Category Category { get; set; }
    public Publisher Publisher { get; set; }
}
