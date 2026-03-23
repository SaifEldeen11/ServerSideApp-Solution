using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.PerformanceRecord
{
    public class UpdatePerformanceRecordDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid record ID is required.")]
        public int Id { get; set; }

        [Range(0.01, 9999.99, ErrorMessage = "Time must be between 0.01 and 9999.99 seconds.")]
        public decimal TimeInSeconds { get; set; }

        [Required(ErrorMessage = "Recorded date is required.")]
        public DateTime RecordedDate { get; set; }

        [MaxLength(500, ErrorMessage = "Comments cannot exceed 500 characters.")]
        public string? Comments { get; set; }
    }
}