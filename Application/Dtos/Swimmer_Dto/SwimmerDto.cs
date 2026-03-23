using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Swimmer_Dto
{
    public class SwimmerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public CompetitionReadiness CompetitionReadiness { get; set; }
        public string CompetitionReadinessName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
    }
}
