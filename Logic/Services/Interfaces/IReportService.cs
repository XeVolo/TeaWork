using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateReport(ReportForm reportForm, int projectId);
    }
}
