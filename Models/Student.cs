using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Email { get; set; }

        // Initialize collections to avoid required validation errors
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<BillingMaster> BillingMasters { get; set; } = new List<BillingMaster>();
    }
}
