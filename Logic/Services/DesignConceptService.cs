using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Logic.Dto;
using Microsoft.EntityFrameworkCore;

namespace TeaWork.Logic.Services
{
    public class DesignConceptService : IDesignConceptService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        public DesignConceptService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = new UserIdentity(context, authenticationStateProvider);
        }
        public async Task Add(DesignConceptDto designConceptData, int projectId)
        {
            ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
            try
            {

                OwnDesignConcept designConcept = new OwnDesignConcept
                {
                    CreationDate = DateTime.Now,
                    Title = designConceptData.Title,
                    Description = designConceptData.Description,
                    UserId = currentUser.Id,
                    IsDeleted=false,
                    ProjectId= projectId,
                };
                _context.OwnDesignConcepts.Add(designConcept);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<OwnDesignConcept>> GetDesignConcepts(int projectId)
        {
            try
            {
                var designconcepts = await _context.OwnDesignConcepts
                    .Where(x => x.ProjectId == projectId)
                    .Include(x => x.User)
                    .Include(x =>x.DesignConceptComments)
                        .ThenInclude(pm => pm.User)
                    .ToListAsync();

                return designconcepts;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task AddComment(DesignConceptDto designCommentData, int designConceptId)
        {
            ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
            try
            {

                DesignConceptComment designComment = new DesignConceptComment
                {
                    CreationDate = DateTime.Now,
                    Title = "Comment",
                    Description = designCommentData.Description,
                    OwnDesignConceptId=designConceptId,
                    UserId = currentUser.Id,
                    IsDeleted = false,
                };
                _context.DesignConceptComments.Add(designComment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
