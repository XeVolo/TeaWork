using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TeaWork.Logic.DbContextFactory;
using TeaWork.Data.Enums;
using System.Threading.Tasks;
using TeaWork.Components.Notifications;

namespace TeaWork.Logic.Services
{
    public class TaskService : ITaskService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        public TaskService(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity =userIdentity;
        }
        public async Task Add(ProjectTaskAddDto taskData, int projectId)
        {
            
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var project = _context.Projects.FirstOrDefault(m => m.Id == projectId);
                if (project != null)
                {
                    ProjectTask projectTask = new ProjectTask
                    {
                        CreationDate = (DateTime)taskData.Start!,
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
            
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var project = _context.Projects.FirstOrDefault(m => m.Id == projectId);
                var projecttaks = await _context.ProjectTasks
                    .Where(x => x.ToDoListId == project.ToDoListId)
                    .Include(x => x.TaskComments)
                        .ThenInclude(y => y.User)
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
        public async Task<List<ProjectTask>> GetMyProjectTasks()
        {

            try
            {
                List<ProjectTask> myTasks = new List<ProjectTask>();
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

                
                var projecttaks = await _context.TaskDistributions
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .ToListAsync();
                foreach (var task in projecttaks) 
                {
                    var projecttask = await _context.ProjectTasks.FirstOrDefaultAsync(x => x.Id == task.TaskId);
                    if (projecttask != null)
                    {
                        myTasks.Add(projecttask);
                    }
                }

                return myTasks;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<int> GetProjectId(int taskId)
        {

            try
            {
                using var _context = _dbContextFactory.CreateDbContext();

                var toDoListId = await _context.ProjectTasks
                    .Where(x => x.Id == taskId)
                    .Select(x => x.ToDoListId)
                    .FirstOrDefaultAsync();

                var project = await _context.Projects
                    .Where(x => x.ToDoListId == toDoListId)
                    .FirstOrDefaultAsync();

                return project.Id;
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
                using var _context = _dbContextFactory.CreateDbContext();
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
        public async Task AddComment(DesignConceptDto taskCommentData, int taskId)
        {
            
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                TaskComment taskComment = new TaskComment
                {
                    CreationDate = DateTime.Now,
                    Title = "Comment",
                    Description = taskCommentData.Description,
                    TaskId = taskId,
                    UserId = currentUser.Id,
                    IsDeleted = false,
                };
                _context.TaskComments.Add(taskComment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task ChangePriorityTask(int projectTaskId, TaskPriority priority)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var projecttask = await _context.ProjectTasks.FirstOrDefaultAsync(x => x.Id == projectTaskId);
                if (projecttask != null)
                {
                    projecttask.Priority = priority;
                    _context.Attach(projecttask).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task ChangeStateTask(int projectTaskId, TaskState state)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var projecttask = await _context.ProjectTasks.FirstOrDefaultAsync(x => x.Id == projectTaskId);
                if (projecttask != null)
                {
                    projecttask.State = state;
                    _context.Attach(projecttask).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
