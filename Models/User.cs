using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GameShop.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
