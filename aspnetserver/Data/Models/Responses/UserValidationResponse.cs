namespace aspnetserver.Data.Models.Responses
{
    public class UserValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Reason { get; set; }
    }
}
