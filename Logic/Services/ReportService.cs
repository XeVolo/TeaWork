using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Logic.DbContextFactory;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Threading.Tasks;
using TeaWork.Logic.Dto;
using TeaWork.Data.Models;

namespace TeaWork.Logic.Services
{
    public class ReportService : IReportService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        private readonly IConversationService _conversationService;
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly IDesignConceptService _designConceptService;


        public ReportService(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = userIdentity;
            _conversationService = new ConversationService(dbContextFactory, authenticationStateProvider, userIdentity);
            _projectService = new ProjectService(dbContextFactory, authenticationStateProvider, userIdentity);
            _taskService = new TaskService(dbContextFactory, authenticationStateProvider, userIdentity);
            _designConceptService = new DesignConceptService(dbContextFactory, authenticationStateProvider, userIdentity);
        }

        public async Task<byte[]> GenerateReport(ReportForm reportForm, int projectId)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var project = await _projectService.GetProjectById(projectId);
                var projectTasks = await _taskService.GetProjectTasks(projectId);
                var projectDesignConcept = await _designConceptService.GetDesignConcepts(projectId);


                double yPoint = 40;
                gfx.DrawString("A report summarizing work on the project", new XFont("Arial", 20, XFontStyle.Bold), XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 30;

                gfx.DrawString($"Title: {project.Title}", new XFont("Arial", 12), XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 20;

                gfx.DrawString($"Description: {project.Description}", new XFont("Arial", 12), XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 20;

                gfx.DrawString($"Start Date: {project.StartDate:yyyy-MM-dd}", new XFont("Arial", 12), XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 20;

                gfx.DrawString($"Deadline: {project.Deadline:yyyy-MM-dd}", new XFont("Arial", 12), XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 30;

                gfx.DrawString("Tasks:", new XFont("Arial", 14, XFontStyle.Bold), XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 20;
                foreach (var task in projectTasks)
                {
                    gfx.DrawString($"- {task.Title} (Deadline: {task.Deadline:yyyy-MM-dd})", new XFont("Arial", 12), XBrushes.Black, new XPoint(60, yPoint));
                    yPoint += 15;

                    var assignedUsers = task.TasksDistributions?
                        .Where(td => td.User != null)
                        .Select(td => td.User!.UserName)
                        .ToList();

                    if (assignedUsers != null && assignedUsers.Count > 0)
                    {
                        gfx.DrawString($"  Assigned Users: {string.Join(", ", assignedUsers)}", new XFont("Arial", 12, XFontStyle.Italic), XBrushes.Black, new XPoint(80, yPoint));
                        yPoint += 20;
                    }

                    
                    if (yPoint > page.Height - 50)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPoint = 40;
                    }
                }
                yPoint += 50;
                gfx.DrawString("User participation in the implementation of tasks:", new XFont("Arial", 14), XBrushes.Black, new XPoint(40, yPoint));
                var taskContributions = GetTaskContributions(projectTasks);
                DrawPieChart(gfx, taskContributions, new XPoint(200, yPoint + 100));
                yPoint += 230;

                
                gfx.DrawString("User participation in concept creation:", new XFont("Arial", 14), XBrushes.Black, new XPoint(40, yPoint));
                var conceptContributions = GetConceptContributions(projectDesignConcept);
                DrawPieChart(gfx, conceptContributions, new XPoint(200, yPoint + 100));

                
                using var stream = new MemoryStream();
                document.Save(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        private void DrawPieChart(XGraphics gfx, Dictionary<string, int> contributions, XPoint center)
        {
            var total = contributions.Values.Sum();
            double startAngle = 0;


            var colors = new[]
                {
                    XColor.FromArgb(255, 255, 153, 153),
                    XColor.FromArgb(255, 153, 255, 153),
                    XColor.FromArgb(255, 153, 204, 255), 
                    XColor.FromArgb(255, 255, 255, 153), 
                    XColor.FromArgb(255, 204, 153, 255) 
                };

            int radius = 80;

            int i = 0;
            foreach (var entry in contributions)
            {
                var sweepAngle = (entry.Value / (double)total) * 360;

                
                var rect = new XRect(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                var brush = new XSolidBrush(colors[i % colors.Length]);
                gfx.DrawPie(brush, rect, startAngle, sweepAngle);

                double midAngle = startAngle + sweepAngle / 2;
                double labelX = center.X + (radius + 20) * Math.Cos(midAngle * Math.PI / 180);
                double labelY = center.Y + (radius + 20) * Math.Sin(midAngle * Math.PI / 180);

                
                gfx.DrawString(entry.Key, new XFont("Arial", 10), XBrushes.Black, new XPoint(labelX, labelY), XStringFormats.Center);

                startAngle += sweepAngle;
                i++;
            }
        }
        private Dictionary<string, int> GetTaskContributions(List<ProjectTask> tasks)
        {
            var contributions = new Dictionary<string, int>();

            foreach (var task in tasks)
            {
                if (task.TasksDistributions == null) continue;

                foreach (var distribution in task.TasksDistributions)
                {
                    if (distribution.User != null)
                    {
                        var userName = distribution.User.UserName ?? "Unknown";
                        if (!contributions.ContainsKey(userName))
                            contributions[userName] = 0;

                        contributions[userName]++;
                    }
                }
            }

            return contributions;
        }

        private Dictionary<string, int> GetConceptContributions(List<OwnDesignConcept> concepts)
        {
            var contributions = new Dictionary<string, int>();

            foreach (var concept in concepts)
            {
                if (concept.User != null)
                {
                    var userName = concept.User.UserName ?? "Unknown";
                    if (!contributions.ContainsKey(userName))
                        contributions[userName] = 0;

                    contributions[userName]++;
                }

                if (concept.DesignConceptComments == null) continue;

                foreach (var comment in concept.DesignConceptComments)
                {
                    if (comment.User != null)
                    {
                        var userName = comment.User.UserName ?? "Unknown";
                        if (!contributions.ContainsKey(userName))
                            contributions[userName] = 0;

                        contributions[userName]++;
                    }
                }
            }

            return contributions;
        }
    }
}
