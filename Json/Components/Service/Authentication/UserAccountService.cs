namespace Json.Components.Service.Authentication
{
    public class UserAccountService
    {
        public List<UserAccount> Users { get; set; } = [];
        private readonly IConfiguration _configuration;
        public UserAccountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _configuration.GetSection("DefaultAccounts").Bind(Users);
        }

        public UserAccount? GetByUserName(string userName)
        {
            return Users.FirstOrDefault(x => x.UserName == userName);
        }
    }
}
