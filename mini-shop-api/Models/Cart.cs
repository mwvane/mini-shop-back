using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class Cart
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ItemId { get; set; }
    }
}
