namespace PostsByMarko.Host.Data.Models.Responses
{
    public class AuthorDetailsResponse
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public AuthorDetailsResponse() { }

        public AuthorDetailsResponse(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            UserName = user.UserName;
        }
    }
}
