using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class ProductDocument
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string DocumentUrl { get; set; }
    }
}
