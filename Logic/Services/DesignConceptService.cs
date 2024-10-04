using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Logic.Dto;

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
        public async Task Add(DesignConceptDto designConceptData, Project project)
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
                    //ProjectId= project.Id,
                };
                _context.OwnDesignConcepts.Add(designConcept);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
