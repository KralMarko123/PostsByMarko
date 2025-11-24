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

        public string Id => requestContext.User.FindFirstValue(ClaimTypes.PrimarySid) ?? string.Empty;

        public string Email => requestContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public IEnumerable<string> Roles => requestContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
    }
}
