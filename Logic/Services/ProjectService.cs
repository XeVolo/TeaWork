using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;

namespace TeaWork.Logic.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        private readonly IConversationService _conversationService;


        public ProjectService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = new UserIdentity(context, authenticationStateProvider);
            _conversationService = new ConversationService(context, authenticationStateProvider);
        }
        public async Task Add(ProjectAddDto projectdata)
        {

            ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

            ToDoList toDoList = new ToDoList();
            try
            {

                _context.ToDoLists.Add(toDoList);
                await _context.SaveChangesAsync();
                Conversation conversation = await _conversationService.AddConversation(ConversationType.GroupChat);
                await _conversationService.AddMember(conversation, currentUser.Id);
                Project project = new Project
                {
                    Title = projectdata.Title,
                    StartDate = DateTime.Now,
                    Deadline = projectdata.Deadline,
                    Description = projectdata.Description,
                    ToDoListId = toDoList.Id,
                    ProjectConversationId = conversation.Id,
                };
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                await AddProjectMember(project, currentUser, ProjectMemberRole.Admin);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
                _context.Projects.Remove(project!);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<Project> GetOne(int id)
        {
            try
            {
                var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
                return project!;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<List<Project>> GetProjects()
        {
            try
            {
                var projects = await _context.Projects.ToListAsync();
                return projects;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<Project>> GetMyProjects()
        {
            List<Project> projects = new List<Project>();
            try
            {

                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var projectMembers = await _context.ProjectMembers.Where(x => x.UserId.Equals(currentUser.Id)).ToListAsync();
                foreach (var projectMember in projectMembers)
                {
                    var project = await _context.Projects.Where(x => x.Id == projectMember.ProjectId).FirstOrDefaultAsync();
                    if (project != null)
                        projects.Add(project);
                }
                return projects;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }


        public async Task Update(Project project, int id)
        {
            try
            {
                var projectToEdit = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
                projectToEdit = project;
                _context.Attach(projectToEdit!).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task AddProjectMember(Project project, ApplicationUser user, ProjectMemberRole role)
        {
            ProjectMember projectMember = new ProjectMember { UserId = user.Id, ProjectId = project.Id, Role = role };
            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();
        }
    }
}
