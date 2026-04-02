using Microsoft.AspNetCore.Identity;

namespace RentACar.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EGN { get; set; } = string.Empty;
    }
}