using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using TeaWork.Data;
using TeaWork.Logic.Services.Interfaces;

namespace TeaWork.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        public UserService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = new UserIdentity(context, authenticationStateProvider);
        }
        public async Task<List<string>> GetUsersEmails()
        {
            try
            {
                var usersemails = await _context.Users
                    .Select(x => x.Email)
                    .ToListAsync();
                return usersemails!;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

        }
        public async Task<string> GetLoggedUserId()
        {

            try
            {
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                return currentUser.Id;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<string> FindUserByEmail(string email)
        {
            try
            {
                var userId = await _context.Users
                    .Where(x => x.Email.Equals(email))
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                return userId;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

        }
    }
}
