using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Publisher
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa  jest wymagana")]
    [StringLength(200, ErrorMessage = "Nazwa nie może mieć więcej niż 100 znaków")]
    public string Name { get; set; }

    // 1 → N
    public ICollection<Game> Games { get; set; }
}