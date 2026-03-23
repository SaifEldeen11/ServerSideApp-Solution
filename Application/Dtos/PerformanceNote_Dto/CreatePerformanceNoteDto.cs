using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.PerformanceNote
{
    public class CreatePerformanceNoteDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid swimmer ID is required.")]
        public int SwimmerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid coach ID is required.")]
        public int CoachId { get; set; }

        [Required(ErrorMessage = "Note is required.")]
        [MaxLength(1000, ErrorMessage = "Note cannot exceed 1000 characters.")]
        public string Note { get; set; } = string.Empty;

        [Required(ErrorMessage = "Note date is required.")]
        public DateTime NoteDate { get; set; }
    }
}