using PostsByMarko.Host.Application.Enums;

namespace PostsByMarko.Host.Application.Requests
{
    public class UpdateUserRolesRequest
    {
        public Guid UserId { get; set; }
        public ActionType ActionType { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
