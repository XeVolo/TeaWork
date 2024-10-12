using TeaWork.Data.Models;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface ITaskService
    {
        Task Add(ProjectTaskAddDto taskdata, int projectId);
        Task<List<ProjectTask>> GetProjectTasks(int projectId);
        Task AddTaskDistribution(int taskId, string userId);
    }
}
