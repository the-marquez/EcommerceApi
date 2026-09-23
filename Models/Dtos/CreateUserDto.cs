
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models.Dtos
{
    public class CreateUserDto
    {
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? UserName { get; set; }    
        
        [Required]
        public string? Password { get; set; }    
        
        [Required]
        public string? Role { get; set; }
    }
}