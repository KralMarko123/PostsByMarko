using System.ComponentModel.DataAnnotations;

namespace aspnetserver.Data.Models.Dtos
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "FirstName is required.")]
        public string? FirstName { get; init; }

        [Required(ErrorMessage = "LastName is required.")]
        public string? LastName { get; init; }

        public string? Email { get; init; }

        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; init; }
    }
}
