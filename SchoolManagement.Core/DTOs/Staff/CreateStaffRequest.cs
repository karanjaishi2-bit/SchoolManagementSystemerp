using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.DTOs.Staff
{
    public class CreateStaffRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Dob { get; set; } = string.Empty;
        public string StaffId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? Gender { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
public string? Photo { get; set; }
        public string Status { get; set; } = "Active";
    }
}