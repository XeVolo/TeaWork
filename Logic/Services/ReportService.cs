using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Logic.DbContextFactory;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Threading.Tasks;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services
{
    public class ReportService : IReportService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        private readonly IConversationService _conversationService;


        public ReportService(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = userIdentity;
            _conversationService = new ConversationService(dbContextFactory, authenticationStateProvider, userIdentity);
        }

        public async Task<byte[]> GenerateReport(ReportForm reportForm, int projectId)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Nagłówek
            gfx.DrawString("Podsumowanie projektu", new XFont("Arial", 20, XFontStyle.Bold), XBrushes.Black, new XPoint(40, 40));

            // Lista zadań i użytkowników
            double yPoint = 70;
            gfx.DrawString("Zadania:", new XFont("Arial", 14), XBrushes.Black, new XPoint(40, yPoint));
            gfx.DrawString($"- Blr blr (Użytkownicy: Janush)", new XFont("Arial", 12), XBrushes.Black, new XPoint(60, yPoint));
            /*
            foreach (var task in tasks)
            {
                yPoint += 20;
                gfx.DrawString($"- {task.Title} (Użytkownicy: {string.Join(", ", task.AssignedUsers)})", new XFont("Arial", 12), XBrushes.Black, new XPoint(60, yPoint));
            }
            /*
            // Wykresy
            // Wykres 1: Udział w zadaniach
            yPoint += 40;
            gfx.DrawString("Udział użytkowników w realizacji zadań:", new XFont("Arial", 14), XBrushes.Black, new XPoint(40, yPoint));
            DrawPieChart(gfx, users.Select(u => u.TaskContribution).ToList(), new XPoint(100, yPoint + 30));

            // Wykres 2: Udział w tworzeniu koncepcji
            yPoint += 150;
            gfx.DrawString("Udział użytkowników w tworzeniu koncepcji:", new XFont("Arial", 14), XBrushes.Black, new XPoint(40, yPoint));
            DrawPieChart(gfx, users.Select(u => u.ConceptContribution).ToList(), new XPoint(100, yPoint + 30));

            */
            // Zapisanie PDF do strumienia
            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }

        private void DrawPieChart(XGraphics gfx, List<int> values, XPoint center)
        {
            var total = values.Sum();
            double startAngle = 0;

            var colors = new[] { XBrushes.Red, XBrushes.Green, XBrushes.Blue, XBrushes.Yellow, XBrushes.Purple };
            for (int i = 0; i < values.Count; i++)
            {
                var sweepAngle = (values[i] / (double)total) * 360;
                gfx.DrawPie(colors[i % colors.Length], new XRect(center.X, center.Y, 100, 100), startAngle, sweepAngle);
                startAngle += sweepAngle;
            }
        }
    }
}
