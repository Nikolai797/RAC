using System.ComponentModel.DataAnnotations;

namespace RentACar.Web1.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "EGN")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "EGN must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "EGN must contain only digits.")]
        public string EGN { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}