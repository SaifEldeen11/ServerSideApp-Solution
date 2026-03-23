using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PerformanceNote : BaseEntity
    {
        public int SwimmerId { get; set; }
        public int CoachId { get; set; }
        public string Note { get; set; } = default!;
        public DateTime NoteDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Swimmer Swimmer { get; set; } = null!;
        public Coach Coach { get; set; } = null!;
    }
}
