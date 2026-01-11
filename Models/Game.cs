using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GameShop.Models;

public class Game
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tytuł gry jest wymagany")]
    [StringLength(200, ErrorMessage = "Tytuł nie może mieć więcej niż 200 znaków")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Opis gry jest wymagany")]
    [StringLength(2000, ErrorMessage = "Opis nie może mieć więcej niż 2000 znaków")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Cena jest wymagana")]
    [Range(0.00, double.MaxValue, ErrorMessage = "Cena nie może być mniejsza niż zero")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Data premiery gry jest wymagana")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Ilość sztuk w magazynie jest wymagana")]
    [Range(0.00, int.MaxValue, ErrorMessage = "Ilość sztuk w magazynie nie może być mniejsza niż zero")]
    public int Stock { get; set; }

    // FK
    [Required(ErrorMessage = "Kategoria jest wymagana")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Wydawca jest wymagany")]
    public int PublisherId { get; set; }

    // Navigation
    public Category Category { get; set; }
    public Publisher Publisher { get; set; }

    [Required(ErrorMessage = "Platforma gry jest wymagana")]
    public Platform GamePlatform { get; set; }
}
