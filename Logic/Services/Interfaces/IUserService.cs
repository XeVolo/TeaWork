using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> GetUsersEmails();
        Task<string> FindUserByEmail(string email);
        Task<string> GetLoggedUserId();
        Task<List<UserDto>> GetProjectUsers(int projectId);
        Task<string> FindUserEmailById(string userId);
    }
}
