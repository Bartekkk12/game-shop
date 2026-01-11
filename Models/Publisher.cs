using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Publisher
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Country { get; set; }

    // 1 → N
    public ICollection<Game> Games { get; set; }
}
