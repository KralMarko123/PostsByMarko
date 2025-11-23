namespace PostsByMarko.Host.Application.Requests
{
    public class UpdatePostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool Hidden { get; set; } = false;
    }
}
