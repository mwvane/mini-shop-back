using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int Quantitiy { get; set; }  
    }
}
