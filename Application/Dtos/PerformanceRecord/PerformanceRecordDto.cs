using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.PerformanceRecord
{
    public class PerformanceRecordDto
    {
        public int Id { get; set; }
        public int SwimmerId { get; set; }
        public string SwimmerName { get; set; } = string.Empty;
        public EventDistance Distance { get; set; }
        public decimal TimeInSeconds { get; set; }
        public DateTime RecordedDate { get; set; }
        public int RecordedByCoachId { get; set; }
        public string RecordedByCoachName { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}
