using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Coach : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<PerformanceNote> PerformanceNotes { get; set; } = new List<PerformanceNote>();
        public ICollection<PerformanceRecord> PerformanceRecords { get; set; } = new List<PerformanceRecord>();
        // Computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}
