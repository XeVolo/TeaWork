using TeaWork.Data.Enums;
using TeaWork.Data;
using TeaWork.Data.Models;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IProjectService
    {
        Task Add(ProjectAddDto projectdata);
        Task Delete(int id);
        Task Update(Project project, int id);
        Task<Project> GetProjectById(int id);
        Task<List<Project>> GetProjects();
        Task<List<Project>> GetMyProjects();
        Task AddProjectMember(Project project, ApplicationUser user, ProjectMemberRole role);
    }
}
