using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Logic.Dto;
using Microsoft.EntityFrameworkCore;
using TeaWork.Logic.DbContextFactory;

namespace TeaWork.Logic.Services
{
    public class DesignConceptService : IDesignConceptService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        public DesignConceptService(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity =userIdentity;
        }
        public async Task Add(DesignConceptDto designConceptData, int projectId)
        {
            
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
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
                using var _context = _dbContextFactory.CreateDbContext();
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
            
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
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
