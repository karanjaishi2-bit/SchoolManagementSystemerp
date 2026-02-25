using SchoolManagement.Core.Entities.Fees;
using SchoolManagement.Core.Entities.Results;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities.Students
{
    public class Student : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Dob { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public string Status { get; set; } = "Active";
        public string? Section { get; set; }
        // ✅ Add this line
        public string Gender { get; set; } = "Other";
        [Column(TypeName = "nvarchar(MAX)")]
public string? Photo { get; set; }

        // Navigation Properties
        public ICollection<Grade>? Grades { get; set; }
  
        public ICollection<Fee>? Fees { get; set; }
    }
}
