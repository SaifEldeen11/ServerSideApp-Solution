using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Swimmer_Dto
{
    public class SwimmerDashboardDto
    {
        public SwimmerDto Swimmer { get; set; } = null!;
        public List<DistancePerformanceDto> PerformanceByDistance { get; set; } = new();
        public List<RecentPerformanceDto> RecentPerformances { get; set; } = new();
        public List<PerformanceNoteDto> RecentNotes { get; set; } = new();
        public PerformanceStatisticsDto Statistics { get; set; } = null!;
    }
    public class DistancePerformanceDto
    {
        public int Distance { get; set; }
        public decimal? BestTime { get; set; }
        public decimal? AverageTime { get; set; }
        public decimal? LatestTime { get; set; }
        public DateTime? BestTimeDate { get; set; }
        public int TotalAttempts { get; set; }
        public decimal? ImprovementPercentage { get; set; }
        public List<PerformanceRecordSimpleDto> History { get; set; } = new();
    }

    public class RecentPerformanceDto
    {
        public int Id { get; set; }
        public int Distance { get; set; }
        public decimal TimeInSeconds { get; set; }
        public DateTime RecordedDate { get; set; }
        public string RecordedByCoachName { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }

    public class PerformanceRecordSimpleDto
    {
        public int Id { get; set; }
        public decimal TimeInSeconds { get; set; }
        public DateTime RecordedDate { get; set; }
    }

    public class PerformanceNoteDto
    {
        public int Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime NoteDate { get; set; }
        public string CoachName { get; set; } = string.Empty;
    }

    public class PerformanceStatisticsDto
    {
        public int TotalPerformances { get; set; }
        public int TotalNotes { get; set; }
        public decimal? OverallAverageTime { get; set; }
        public string? MostImprovedDistance { get; set; }
        public decimal? MostImprovedPercentage { get; set; }
    }
}
