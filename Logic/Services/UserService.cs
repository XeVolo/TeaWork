using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using TeaWork.Data;
using TeaWork.Logic.DbContextFactory;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;

namespace TeaWork.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        public UserService(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = userIdentity;
        }
        public async Task<List<string>> GetUsersEmails()
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
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
                using var _context = _dbContextFactory.CreateDbContext();
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
                using var _context = _dbContextFactory.CreateDbContext();
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
        public async Task<List<UserDto>> GetProjectUsers(int projectId)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                List<UserDto> Users = new List<UserDto>();
                var projectmembers = await _context.ProjectMembers
                    .Where(x => x.ProjectId == projectId)
                    .ToListAsync();
                foreach (var projectmember in projectmembers)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x=>x.Id.Equals(projectmember.UserId));
                    if (user != null)
                    {
                        UserDto userDto = new UserDto
                        {
                            Id = user.Id,
                            Name = user.UserName,
                        };
                        Users.Add(userDto);
                    }
                }
                return Users;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
