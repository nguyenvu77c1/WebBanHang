using System.ComponentModel.DataAnnotations;

namespace Lab03.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public string? Brand { get; set; }
        [Range(0,100000000)]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int ConfigId { get; set; }
        public Config? Configs { get; set; }
    }
}
