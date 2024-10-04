using TeaWork.Data.Models;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface ITaskService
    {
        Task Add(ProjectTaskAddDto taskdata, Project project);
    }
}
