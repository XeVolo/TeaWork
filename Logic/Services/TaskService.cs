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
        private readonly IUserIdentity _userIdentity;
        private readonly ILogger<TaskService> _logger;
        public TaskService(
            IDbContextFactory dbContextFactory, 
            IUserIdentity userIdentity,
            ILogger<TaskService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _userIdentity =userIdentity;
            _logger = logger;
        }
        public async Task Add(ProjectTaskAddDto taskData, int projectId)
        {
            
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
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
                _logger.LogError(ex, "Failed to add project task.");
                throw;
            }
        }
        public async Task<List<ProjectTask>> GetProjectTasks(int projectId)
        {
            
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var project = _context.Projects.FirstOrDefault(m => m.Id == projectId);
                var projectTaks = await _context.ProjectTasks
                    .Where(x => x.ToDoListId == project.ToDoListId)
                    .Include(x => x.TaskComments)
                        .ThenInclude(y => y.User)
                    .Include(x => x.TasksDistributions)
                        .ThenInclude(y => y.User)
                    .ToListAsync();
                return projectTaks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get project tasks.");
                throw;
            }
        }
        public async Task<List<ProjectTask>> GetMyProjectTasks()
        {

            try
            {
                List<ProjectTask> myTasks = new List<ProjectTask>();
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

                
                var projecttaks = await _context.TaskDistributions
                    .Where(x => x.UserId!.Equals(currentUser.Id))
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
                _logger.LogError(ex, "Failed to get my project tasks.");
                throw;
            }
        }
        public async Task<int> GetProjectId(int taskId)
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();

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
                _logger.LogError(ex, "Failed to get projectId.");
                throw;
            }
        }
        public async Task AddTaskDistribution(int taskId, string userId)
        {
            
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
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
                _logger.LogError(ex, "Failed to add task distribution.");
                throw;
            }
        }
        public async Task AddComment(DesignConceptDto taskCommentData, int taskId)
        {
            
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
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
                _logger.LogError(ex, "Failed to add comment.");
                throw;
            }
        }
        public async Task ChangePriorityTask(int projectTaskId, TaskPriority priority)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
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
                _logger.LogError(ex, "Failed to change task priority.");
                throw;
            }
        }
        public async Task ChangeStateTask(int projectTaskId, TaskState state)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
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
                _logger.LogError(ex, "Failed to change task state.");
                throw;
            }
        }

        public async Task<int> AddPrivateTask(DateTime start,DateTime end, string title, string? description)
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

                PrivateTask privateTask = new PrivateTask
                {
                    Start = start,
                    End = end,
                    Title = title,
                    Description = description,
                    UserId = currentUser.Id,
                };
                _context.PrivateTasks.Add(privateTask);
                await _context.SaveChangesAsync();
                return privateTask.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add private task.");
                throw;
            }
        }
        public async Task EditPrivateTask(int taskId, DateTime start, DateTime end, string title, string? description)
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var privateTaskToEdit = await _context.PrivateTasks.FirstOrDefaultAsync(m => m.Id == taskId);
                if (privateTaskToEdit != null)
                {
                    privateTaskToEdit.Title = title;
                    privateTaskToEdit.Description = description;
                    privateTaskToEdit.Start = start;
                    privateTaskToEdit.End = end;
                    _context.Attach(privateTaskToEdit!).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to edit private task.");
                throw;
            }
        }
        public async Task<List<PrivateTask>> GetMyPrivateTasks()
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();


                var privateTasks = await _context.PrivateTasks
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .ToListAsync();       
                return privateTasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get my private tasks.");
                throw;
            }
        }

    }
}
