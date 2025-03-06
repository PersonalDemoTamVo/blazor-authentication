using System.Security.Claims;
using Json.Components.Entities;
using Microsoft.AspNetCore.Components.Authorization;

namespace Json.Components.Service.Authen
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        public DefaultAccount CurrentAccount { get; private set; } = new();
        private readonly CustomAuthenticationService _authService;
        private AuthenticationState _authenticationState;

        public CustomAuthStateProvider(CustomAuthenticationService authService)
        {
            _authService = authService;
            AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
            //_authService.UserChanged += OnUserChanged;
        }

        private void OnUserChanged(ClaimsPrincipal newUser)
        {
            _authenticationState = new AuthenticationState(newUser);
            NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
        }

        public async Task LoginAsync(string username, string password)
        {
            var principal = new ClaimsPrincipal();
            var user = await _authService.ValidateUser(username, password);
            CurrentAccount = user;

            if (user is not null)
            {
                principal = user.ToClaimsPrincipal();
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var principal = new ClaimsPrincipal();
            var user = await _authService.FetchUserFromBrowserAsync();

            if (user is not null)
            {
                var authenticatedUser = await _authService.ValidateUser(user.Username, user.Password);
                CurrentAccount = authenticatedUser;

                if (authenticatedUser is not null)
                {
                    principal = authenticatedUser.ToClaimsPrincipal();
                }
            }

            return new AuthenticationState(principal);
        }

        public async Task LogoutAsync()
        {
            await _authService.ClearBrowserUserDataAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
        }

        private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
        {
            var authenticationState = await task;

            if (authenticationState is not null)
            {
                CurrentAccount = DefaultAccount.FromClaimsPrincipal(authenticationState.User);
            }
        }

        public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
    }
}
