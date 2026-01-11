using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    // 1 → N
    public ICollection<Game> Games { get; set; }
}
