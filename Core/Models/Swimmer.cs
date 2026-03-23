using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Swimmer : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public CompetitionReadiness CompetitionReadiness { get; set; } = CompetitionReadiness.NotReady;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public ICollection<PerformanceRecord> PerformanceRecords { get; set; } = new List<PerformanceRecord>();
        public ICollection<PerformanceNote> PerformanceNotes { get; set; } = new List<PerformanceNote>();

        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.UtcNow.Year - DateOfBirth.Year;
    }
}
