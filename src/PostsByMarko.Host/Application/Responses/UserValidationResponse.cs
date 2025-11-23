namespace PostsByMarko.Host.Application.Responses
{
    public class UserValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Reason { get; set; } = string.Empty;
    }
}
