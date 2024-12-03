using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TeaWork.Data;
using TeaWork.Logic.DbContextFactory;
using TeaWork.Logic.Services.Interfaces;

namespace TeaWork.Logic.Services
{
    public class UserIdentity : IUserIdentity
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger<UserIdentity> _logger;

        public UserIdentity(
            IDbContextFactory dbContextFactory, 
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<UserIdentity> logger)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        public async Task<ApplicationUser> GetLoggedUser()
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity!.Name);
                if (currentUser == null)
                {
                    _logger.LogError("Failed to get logged user.");
                    throw new InvalidOperationException("Unknown user");
                }
                return currentUser!;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get logged user.");
                throw;
            }
        }
    }
}
