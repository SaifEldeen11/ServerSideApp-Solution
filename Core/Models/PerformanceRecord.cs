using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PerformanceRecord : BaseEntity
    {
        public int SwimmerId { get; set; }
        public EventDistance Distance { get; set; }
        public decimal TimeInSeconds { get; set; }
        public DateTime RecordedDate { get; set; }
        public int RecordedByCoachId { get; set; }
        public string? Comments { get; set; }

        // Navigation properties
        public Swimmer Swimmer { get; set; } = null!;
        public Coach RecordedByCoach { get; set; } = null!;
    }
}
