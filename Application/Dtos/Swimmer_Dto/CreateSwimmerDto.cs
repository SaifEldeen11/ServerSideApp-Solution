using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Swimmer_Dto
{
    public class CreateSwimmerDto
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

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number is not valid.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Join date is required.")]
        public DateTime JoinDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid team ID is required.")]
        public int? TeamId { get; set; }
    }
}