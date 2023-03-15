using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace mini_shop_api.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [AllowNull]
        public string? Document { get; set; }
    }
}
