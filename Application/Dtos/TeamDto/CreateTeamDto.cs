using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.TeamDto
{
    public class CreateTeamDto
    {
        [Required(ErrorMessage = "Team name is required.")]
        [MaxLength(100, ErrorMessage = "Team name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A valid coach ID is required.")]
        public int CoachId { get; set; }
    }
}