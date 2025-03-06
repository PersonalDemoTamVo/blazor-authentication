using System.Security.Claims;

namespace Json.Components.Entities
{
    public class DefaultAccount
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(
    [
        new (ClaimTypes.Name, Username),
        new (ClaimTypes.Hash, Password),
        new (ClaimTypes.Role, Role),
    ], "demoAuthen"));

        public static DefaultAccount FromClaimsPrincipal(ClaimsPrincipal principal) => new()
        {
            Username = principal.FindFirst(ClaimTypes.Name)?.Value ?? "",
            Password = principal.FindFirst(ClaimTypes.Hash)?.Value ?? "",
            Role = principal.FindFirst(ClaimTypes.Role)?.Value ?? "",
        };
    }
}