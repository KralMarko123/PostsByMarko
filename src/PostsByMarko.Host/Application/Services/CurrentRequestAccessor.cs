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

        public HttpContext Context => contextAccessor.HttpContext!;

        public Guid Id => Guid.TryParse(Context.User.FindFirstValue(ClaimTypes.PrimarySid), out var guid) ? guid : Guid.Empty;

        public string Email => Context.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        public IEnumerable<string> Roles => Context.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
    }
}
