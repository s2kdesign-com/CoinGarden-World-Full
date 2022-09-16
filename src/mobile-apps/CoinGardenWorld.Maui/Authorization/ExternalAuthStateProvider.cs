using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoinGardenWorld.Maui.Authorization
{
    public class ExternalAuthStateProvider : AuthenticationStateProvider
    {
        private AuthenticationState currentUser;

        public ExternalAuthStateProvider(ExternalAuthService service)
        {
            currentUser = new AuthenticationState(service.CurrentUser);

            service.UserChanged += (newUser) =>
            {
                currentUser = new AuthenticationState(newUser);
                NotifyAuthenticationStateChanged(Task.FromResult(currentUser));
            };
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(currentUser);
    }

    public class ExternalAuthService
    {
        public event Action<ClaimsPrincipal>? UserChanged;
        private ClaimsPrincipal? currentUser;

        public ClaimsPrincipal CurrentUser
        {
            get { return currentUser ?? new(); }
            set
            {
                currentUser = value;

                if (UserChanged is not null)
                {
                    UserChanged(currentUser);
                }
            }
        }
    }
}
