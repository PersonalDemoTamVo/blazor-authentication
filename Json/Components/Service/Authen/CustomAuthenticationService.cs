using System.Security.Claims;
using Json.Components.Entities;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Json.Components.Service.Authen
{
    public class CustomAuthenticationService
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly IConfiguration _configuration;
        public event Action<ClaimsPrincipal>? UserChanged;
        private readonly IJSRuntime _jsRuntime;
        public List<DefaultAccount> DefaultAccounts { get; set; } = [];
        //  private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly string _storageKey = "demo";

        public CustomAuthenticationService(IConfiguration configuration, ProtectedLocalStorage protectedLocalStorage, IJSRuntime jsRuntime)
        {
            _protectedLocalStorage = protectedLocalStorage;
            _configuration = configuration;
            _configuration.GetSection("DefaultAccounts").Bind(DefaultAccounts);
            _jsRuntime = jsRuntime;
        }

        public async Task PersistUserToBrowserAsync(DefaultAccount account)
        {
            string userJson = JsonConvert.SerializeObject(account);
            await _protectedLocalStorage.SetAsync(_storageKey, userJson);
        }

        private bool IsPrerendering()
        {
            // If JSRuntime is IJSInProcessRuntime, we're not prerendering
            // If it's not, we're in prerendering mode
            return !(_jsRuntime is IJSInProcessRuntime);
        }

        public async Task<DefaultAccount?> FetchUserFromBrowserAsync()
        {
            try
            {
                var fetchedUserResult = await _protectedLocalStorage.GetAsync<string>(_storageKey);

                if (fetchedUserResult.Success && !string.IsNullOrEmpty(fetchedUserResult.Value))
                {
                    return JsonConvert.DeserializeObject<DefaultAccount>(fetchedUserResult.Value);
                }

            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("JavaScript interop cannot run during prerendering. Retrying later...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user from browser storage: {ex.Message}");
            }

            return null;
        }

        public async Task<DefaultAccount?> ValidateUser(string username, string password)
        {
            var account = DefaultAccounts.FirstOrDefault(acc => acc.Username == username && acc.Password == password);
            await PersistUserToBrowserAsync(account);
            return account;
        }

        public async Task ClearBrowserUserDataAsync() => await _protectedLocalStorage.DeleteAsync(_storageKey);

    }

}