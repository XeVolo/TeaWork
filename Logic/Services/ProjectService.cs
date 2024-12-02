using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Logic.DbContextFactory;
using System.Data;
using System.Linq.Dynamic.Core;

namespace TeaWork.Logic.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly UserIdentity _userIdentity;
        private readonly IConversationService _conversationService;
        private readonly ILogger<ProjectService> _logger;


        public ProjectService(
            IDbContextFactory dbContextFactory, 
            UserIdentity userIdentity,
            IConversationService conversationService,
            ILogger<ProjectService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _userIdentity = userIdentity;
            _conversationService = conversationService;
            _logger = logger;
        }
        public async Task Add(ProjectAddDto projectData)
        {            
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ToDoList toDoList = new ToDoList();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                _context.ToDoLists.Add(toDoList);
                await _context.SaveChangesAsync();
                Conversation conversation = await _conversationService.AddConversation(ConversationType.GroupChat, projectData.Title!);
                await _conversationService.AddMember(conversation, currentUser.Id);
                Project project = new Project
                {
                    Title = projectData.Title,
                    StartDate = DateTime.Now,
                    Deadline = (DateTime)projectData.Deadline!,
                    Description = projectData.Description,
                    ToDoListId = toDoList.Id,
                    ProjectConversationId = conversation.Id,
                };
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                await AddProjectMember(project, currentUser, ProjectMemberRole.Admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add project.");
                throw;
            }
        }

        public async Task<Project> GetProjectById(int id)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var project = await _context.Projects
                    .Include(x => x.ProjectMembers)
                        .ThenInclude(pm => pm.User)
                    .FirstOrDefaultAsync(x => x.Id == id);
                return project!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get project.");
                throw;
            }
        }
        public async Task<List<Project>> GetMyProjects()
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var projects = await _context.Projects
                    .Where(p => p.ProjectMembers!
                        .Any(pm => pm.UserId == currentUser.Id))
                    .ToListAsync();
                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get projects.");
                throw;
            }
        }

        public async Task AddProjectMember(Project project, ApplicationUser user, ProjectMemberRole role)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ProjectMember projectMember = new ProjectMember 
                { 
                    UserId = user.Id, 
                    ProjectId = project.Id, 
                    Role = role 
                };
                _context.ProjectMembers.Add(projectMember);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add project member.");
                throw;
            }
        }

        public async Task DeleteUserFromProject(string userId, int projectId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();

                var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == projectId);
                if (project != null)
                {
                    var conversationMember = await _context.ConversationMembers
                        .Where(x => x.UserId.Equals(userId))
                        .Where(x => x.ConversationId == project.ProjectConversationId)
                        .FirstOrDefaultAsync();
                    if (conversationMember != null)
                    {
                        _context.ConversationMembers.Remove(conversationMember);
                        await _context.SaveChangesAsync();
                    }
                    var projectMember = await _context.ProjectMembers
                        .Where(x => x.UserId.Equals(userId))
                        .Where(x => x.ProjectId == project.Id)
                        .FirstOrDefaultAsync();
                    if (projectMember != null) 
                    { 
                        _context.ProjectMembers.Remove(projectMember);
                        await _context.SaveChangesAsync();
                    }
                    var projectTasks = await _context.ProjectTasks
                        .Where(x => x.ToDoListId == project.ToDoListId)
                        .Include(x =>x.TasksDistributions)
                        .ToListAsync();
                    List<TaskDistribution> taskDistributions = new List<TaskDistribution>();
                    foreach (var task in projectTasks)
                    {
                        if (task.TasksDistributions != null)
                        {
                            foreach (var taskDistribution in task.TasksDistributions)
                            {
                                if (taskDistribution.UserId.Equals(userId))
                                {
                                    taskDistributions.Add(taskDistribution);
                                }
                            }
                        }
                    }

                    _context.TaskDistributions.RemoveRange(taskDistributions);
                    await _context.SaveChangesAsync();
                    

                    var invitation =await _context.Invitations
                        .Where(x => x.UserId.Equals(userId))
                        .Where(x => x.ProjectId == project.Id)
                        .FirstOrDefaultAsync();
                    _context.Invitations.Remove(invitation);
                    await _context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove user from project.");
                throw;
            }
        }
        public async Task<bool> CheckUserAccess(int projectId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

                var projectMember = await _context.ProjectMembers
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .Where(x => x.ProjectId == projectId)
                    .FirstOrDefaultAsync();

                return projectMember != null;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user access.");
                throw;
            }
        }

    }
}
