using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Models
{
    public class BillingItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BillingMasterId { get; set; }

        [ForeignKey("BillingMasterId")]
        public BillingMaster? BillingMaster { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
