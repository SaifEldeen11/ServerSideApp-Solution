using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CoachId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Coach Coach { get; set; } = null!;
        public ICollection<Swimmer> Swimmers { get; set; } = new List<Swimmer>();
    }
}
