﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa kategori jest wymagana")]
    [StringLength(200, ErrorMessage = "Nazwa ketegori nie może mieć więcej niż 200 znaków")]
    public string Name { get; set; }

    // 1 → N
    public ICollection<Game> Games { get; set; }
}