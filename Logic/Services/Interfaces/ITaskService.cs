using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface ITaskService
    {
        Task Add(ProjectTaskAddDto taskdata, int projectId);
        Task<List<ProjectTask>> GetProjectTasks(int projectId);
        Task AddTaskDistribution(int taskId, string userId);
        Task AddComment(DesignConceptDto taskCommentData, int taskId);
        Task<int> GetProjectId(int taskId);
        Task<List<ProjectTask>> GetMyProjectTasks();
        Task ChangePriorityTask(int projectTaskId, TaskPriority priority);
        Task ChangeStateTask(int projectTaskId, TaskState state);
    }
}
