
using Microsoft.AspNetCore.Identity;

namespace EcommerceApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}