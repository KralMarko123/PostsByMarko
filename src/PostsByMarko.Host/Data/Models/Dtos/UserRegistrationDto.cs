
namespace PostsByMarko.Host.Data.Models.Dtos
{
    public class UserRegistrationDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public UserRegistrationDto() { }

        public UserRegistrationDto(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

    }
}
