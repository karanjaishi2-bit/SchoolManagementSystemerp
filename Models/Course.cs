using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Teacher is required")]
        public int TeacherId { get; set; }

        // ? MUST be nullable
        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }

        public decimal Fee { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<BillingItem> BillingItems { get; set; } = new List<BillingItem>();
    }
}
