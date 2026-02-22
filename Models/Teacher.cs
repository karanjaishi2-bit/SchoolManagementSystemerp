using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Teacher name is required")]
        public string Name { get; set; }

        public string Email { get; set; }

        // ✅ initialize to avoid validation errors
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
