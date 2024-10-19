using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        public async Task Add(ProjectTaskAddDto taskData, int projectId)
        {
            ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
            try
            {
                var project = _context.Projects.FirstOrDefault(m => m.Id == projectId);
                if (project != null)
                {
                    ProjectTask projectTask = new ProjectTask
                    {
                        CreationDate = DateTime.Now,
                        Deadline = (DateTime)taskData.Deadline!,
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
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<ProjectTask>> GetProjectTasks(int projectId)
        {
            ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
            try
            {
                var project = _context.Projects.FirstOrDefault(m => m.Id == projectId);
                var projecttaks = await _context.ProjectTasks
                    .Where(x => x.ToDoListId == project.ToDoListId)
                    .Include(x => x.TasksDistributions)
                        .ThenInclude(y => y.User)
                    .ToListAsync();
                return projecttaks;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task AddTaskDistribution(int taskId, string userId)
        {
            
            try
            {
                var existingTaskDistribution = await _context.TaskDistributions
                        .FirstOrDefaultAsync(x => x.TaskId == taskId && x.UserId == userId);

                if (existingTaskDistribution == null)
                {
                    TaskDistribution taskDistribution = new TaskDistribution
                    {
                        TaskId = taskId,
                        UserId = userId
                    };
                    _context.TaskDistributions.Add(taskDistribution);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
