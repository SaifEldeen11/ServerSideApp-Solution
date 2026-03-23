using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Swimmer_Dto
{
    public class UpdateSwimmerDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid swimmer ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number is not valid.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EnumDataType(typeof(CompetitionReadiness), ErrorMessage = "Invalid competition readiness value.")]
        public CompetitionReadiness CompetitionReadiness { get; set; }

        public bool IsActive { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid team ID is required.")]
        public int? TeamId { get; set; }
    }
}