using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Coach_Dto
{
    public class CreateCoachDto
    {
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

        [Required(ErrorMessage = "Hire date is required.")]
        public DateTime HireDate { get; set; }
    }
}