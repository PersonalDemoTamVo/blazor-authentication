namespace Json.Components.Service.Authentication
{
    public class UserSession
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public DateTime Expiration { get; set; }
    }
}
