using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime LastUpdated { get; set; }
        [Required]
        public string Role { get; set; } = "user";
    }
}
