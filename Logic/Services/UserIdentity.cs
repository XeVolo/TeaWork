using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TeaWork.Data;
using TeaWork.Logic.DbContextFactory;

namespace TeaWork.Logic.Services
{
    public class UserIdentity 
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserIdentity(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<ApplicationUser> GetLoggedUser()
        {
            using var _context = _dbContextFactory.CreateDbContext();
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
