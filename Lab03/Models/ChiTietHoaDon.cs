using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lab03.Models
{
    public class ChiTietHoaDon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int HoaDonId { get; set; }
        [ForeignKey("HoaDonId")]
        [ValidateNever]
        public HoaDon HoaDon { get; set; }
        [Required]
        public int SanPhamId { get; set; }
        [ForeignKey("SanPhamId")]
        [ValidateNever]
        public Product SanPham { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
