using System.ComponentModel.DataAnnotations;

namespace mini_shop_api.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public DateTime ValidDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int CreatedBy { get; set; }

    }
}
