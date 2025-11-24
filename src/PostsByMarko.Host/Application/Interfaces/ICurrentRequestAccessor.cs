namespace PostsByMarko.Host.Application.Interfaces
{
    public interface ICurrentRequestAccessor
    {
        public HttpContext requestContext { get; }
        public string Id { get; }
        public string Email { get; }
        public IEnumerable<string> Roles { get; }
    }
}
