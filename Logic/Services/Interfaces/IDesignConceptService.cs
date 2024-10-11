using TeaWork.Logic.Dto;
using TeaWork.Data.Models;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IDesignConceptService
    {
        Task Add(DesignConceptDto designConceptData, int projectId);
        Task<List<OwnDesignConcept>> GetDesignConcepts(int projectId);
    }
}
