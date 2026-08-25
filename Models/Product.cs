
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0.0m;
        public string ImgUrl { get; set; } = string.Empty;

        [Required] //prod-001-blk-m
        public string SKU { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative value.")]
        public int Stock { get; set; } = 0;

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public required Category Category { get; set; }

    }
}