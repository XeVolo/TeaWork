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
        private readonly ILogger<UserService> _logger;
        public UserService(
            IDbContextFactory dbContextFactory, 
            AuthenticationStateProvider authenticationStateProvider, 
            UserIdentity userIdentity,
            ILogger<UserService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = userIdentity;
            _logger = logger;
        }
        public async Task<string> GetLoggedUserId()
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                return currentUser.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get logged user.");
                throw;
            }
        }
        public async Task<string> FindUserByEmail(string email)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var userId = await _context.Users
                    .Where(x => x.Email.Equals(email))
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                return userId!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user by email.");
                throw;
            }

        }
        public async Task<string> FindUserEmailById(string userId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var userEmail = await _context.Users
                    .Where(x => x.Id.Equals(userId))
                    .Select(x => x.Email)
                    .FirstOrDefaultAsync();
                return userEmail!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get email by userId.");
                throw;
            }

        }
        public async Task<List<UserDto>> GetProjectUsers(int projectId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
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
                _logger.LogError(ex, "Failed to get project users");
                throw;
            }
        }
    }
}
