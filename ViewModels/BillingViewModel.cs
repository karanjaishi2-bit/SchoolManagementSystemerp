using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SchoolManagement.ViewModels
{
    public class BillingViewModel
    {
        public int Id { get; set; }  // Needed for Edit
        public int StudentId { get; set; }
        public List<int> SelectedCourseIds { get; set; } = new List<int>();

        // Billing type (Annual, Monthly, Weekly, Seasonal)
        public string BillingType { get; set; }

        // Dropdown data
        public List<SelectListItem> Students { get; set; } = new List<SelectListItem>();
        public List<CourseItem> Courses { get; set; } = new List<CourseItem>();
        public List<SelectListItem> BillingTypes { get; set; } = new List<SelectListItem>();

        // Constructor to initialize BillingTypes
        public BillingViewModel()
        {
            BillingTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Annual", Text = "Annual" },
                new SelectListItem { Value = "Monthly", Text = "Monthly" },
                new SelectListItem { Value = "Weekly", Text = "Weekly" },
                new SelectListItem { Value = "Seasonal", Text = "Seasonal" }
            };
        }
    }

    public class CourseItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
    }
}
