using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data;

namespace TeaWork.Logic.Services
{
    public class UserIdentity
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserIdentity(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<ApplicationUser> GetLoggedUser()
        {

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity!.Name);
            if (currentUser == null)
            {
                throw new InvalidOperationException("Unknown user");
            }
            return currentUser!;
        }
    }
}
