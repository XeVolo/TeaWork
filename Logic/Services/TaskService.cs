using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;

namespace TeaWork.Logic.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        public TaskService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = new UserIdentity(context, authenticationStateProvider);
        }
        public async Task Add(ProjectTaskAddDto taskData, Project project)
        {
            ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
            try
            {

                ProjectTask projectTask = new ProjectTask
                {
                    CreationDate = DateTime.Now,
                    Deadline = taskData.Deadline,
                    Title = taskData.Title,
                    Description = taskData.Description,
                    State = taskData.State,
                    Priority = taskData.Priority,
                    ToDoListId = project.ToDoListId,
                    UserId = currentUser.Id,
                };
                _context.ProjectTasks.Add(projectTask);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
