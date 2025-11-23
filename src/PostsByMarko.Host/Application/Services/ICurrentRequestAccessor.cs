namespace PostsByMarko.Host.Application.Services
{
    public interface ICurrentRequestAccessor
    {
        public HttpContext requestContext { get; }
        public string Id { get; }
        public string Email { get; }
        public IEnumerable<string> Roles { get; }
    }
}
