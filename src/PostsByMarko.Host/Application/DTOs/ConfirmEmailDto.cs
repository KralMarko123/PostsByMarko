namespace PostsByMarko.Host.Application.DTOs
{
    public class ConfirmEmailDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
