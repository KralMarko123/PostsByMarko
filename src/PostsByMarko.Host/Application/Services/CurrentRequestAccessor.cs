using PostsByMarko.Host.Application.Interfaces;
using System.Security.Claims;

namespace PostsByMarko.Host.Application.Services
{
    public class CurrentRequestAccessor : ICurrentRequestAccessor
    {
        private readonly IHttpContextAccessor contextAccessor;

        public CurrentRequestAccessor(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public HttpContext requestContext => contextAccessor.HttpContext!;

        public Guid Id => Guid.TryParse(requestContext.User.FindFirstValue(ClaimTypes.PrimarySid), out var guid) ? guid : Guid.Empty;

        public string Email => requestContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public IEnumerable<string> Roles => requestContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
    }
}
