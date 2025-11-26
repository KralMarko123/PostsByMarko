namespace PostsByMarko.Host.Application.Interfaces
{
    public interface ICurrentRequestAccessor
    {
        public HttpContext requestContext { get; }
        public Guid Id { get; }
        public string Email { get; }
        public IEnumerable<string> Roles { get; }
    }
}
