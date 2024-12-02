using TeaWork.Data;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IUserIdentity
    {
        Task<ApplicationUser> GetLoggedUser();
    }
}
