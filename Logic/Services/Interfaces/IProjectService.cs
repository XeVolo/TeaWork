using TeaWork.Data.Enums;
using TeaWork.Data;
using TeaWork.Data.Models;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IProjectService
    {
        Task Add(ProjectAddDto projectdata);
        Task<Project> GetProjectById(int id);
        Task<List<Project>> GetMyProjects();
        Task AddProjectMember(Project project, ApplicationUser user, ProjectMemberRole role);
        Task DeleteUserFromProject(string userId, int projectId);
        Task<bool> CheckUserAccess(int projectId);
    }
}
