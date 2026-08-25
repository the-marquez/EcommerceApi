
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models.Dtos
{
    public class CreateProductDto
    {
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal? Price { get; set; }

        [Required]
        public string? ImgUrl { get; set; }

        [Required] //prod-001-blk-m
        public string? SKU { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative value.")]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}