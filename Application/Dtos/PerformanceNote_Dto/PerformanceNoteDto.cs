using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.PerformanceNote
{
    public class PerformanceNoteDto
    {
        public int Id { get; set; }
        public int SwimmerId { get; set; }
        public string SwimmerName { get; set; } = string.Empty;
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime NoteDate { get; set; }
    }
}
