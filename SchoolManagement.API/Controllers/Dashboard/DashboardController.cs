using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.DTOs.Dashboard;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.API.Controllers.Dashboard
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        public async Task<ActionResult> GetDashboardStats()
        {
            try
            {
                var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

                var stats = new DashboardStatsResponse
                {
                    TotalStudents = await _context.Students.CountAsync(s => s.IsActive),
                    TotalTeachers = await _context.Teachers.CountAsync(t => t.IsActive),
                    TotalClasses = await _context.Classes.CountAsync(c => c.IsActive),
                    TodayAttendance = await _context.Attendances.CountAsync(a => a.Date == today && a.Status == "Present"),
                    PendingFees = await _context.Fees
                        .Where(f => f.Status == "Pending" || f.Status == "Overdue")
                        .SumAsync(f => f.Amount)
                };

                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/dashboard/attendance-overview
        [HttpGet("attendance-overview")]
        public async Task<ActionResult> GetAttendanceOverview([FromQuery] string? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.UtcNow.ToString("yyyy-MM-dd");

                var overview = new AttendanceOverview
                {
                    Present = await _context.Attendances.CountAsync(a => a.Date == targetDate && a.Status == "Present"),
                    Absent = await _context.Attendances.CountAsync(a => a.Date == targetDate && a.Status == "Absent"),
                    Leave = await _context.Attendances.CountAsync(a => a.Date == targetDate && a.Status == "Leave")
                };

                return Ok(new { success = true, data = overview });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/dashboard/monthly-enrollments
        [HttpGet("monthly-enrollments")]
        public async Task<ActionResult> GetMonthlyEnrollments([FromQuery] int year = 0)
        {
            try
            {
                var targetYear = year == 0 ? DateTime.UtcNow.Year : year;

                var enrollments = await _context.Enrollments
                    .Where(e => e.EnrolledOn.StartsWith(targetYear.ToString()))
                    .ToListAsync();

                var monthlyData = enrollments
                    .GroupBy(e => e.EnrolledOn.Substring(0, 7)) // Group by YYYY-MM
                    .Select(g => new MonthlyEnrollment
                    {
                        Month = g.Key,
                        Enrollments = g.Count()
                    })
                    .OrderBy(m => m.Month)
                    .ToList();

                return Ok(new { success = true, data = monthlyData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/dashboard/student-report

        // GET: api/dashboard/student-report
        [HttpGet("student-report")]
        public async Task<ActionResult> GetStudentReport()
        {
            try
            {
                var totalStudents = await _context.Students.CountAsync();
                var activeStudents = await _context.Students.CountAsync(s => s.Status == "Active");
                var inactiveStudents = await _context.Students.CountAsync(s => s.Status == "Inactive");

                var byClass = await _context.Students
                    .GroupBy(s => s.Class)
                    .Select(g => new { Class = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Class ?? "Unknown", x => x.Count);

                var bySection = await _context.Students
                    .Where(s => !string.IsNullOrEmpty(s.Section))
                    .GroupBy(s => s.Section)
                    .Select(g => new { Section = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Section ?? "Unknown", x => x.Count);

                var byGender = await _context.Students
                    .Where(s => !string.IsNullOrEmpty(s.Gender))
                    .GroupBy(s => s.Gender)
                    .Select(g => new { Gender = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Gender ?? "Unknown", x => x.Count);

                var report = new
                {
                    totalStudents,
                    activeStudents,
                    inactiveStudents,
                    byClass,
                    bySection,
                    byGender
                };

                return Ok(new { success = true, data = report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
        // GET: api/dashboard/attendance-report
        [HttpGet("attendance-report")]
        public async Task<ActionResult> GetAttendanceReport([FromQuery] string? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.UtcNow.ToString("yyyy-MM-dd");

                var totalPresent = await _context.Attendances.CountAsync(a => a.Date == targetDate && a.Status == "Present");
                var totalAbsent = await _context.Attendances.CountAsync(a => a.Date == targetDate && a.Status == "Absent");
                var totalLeave = await _context.Attendances.CountAsync(a => a.Date == targetDate && a.Status == "Leave");
                var total = totalPresent + totalAbsent + totalLeave;

                var percentage = total > 0 ? (decimal)totalPresent / total * 100 : 0;

                var report = new
                {
                    date = targetDate,
                    totalPresent,
                    totalAbsent,
                    totalLeave,
                    percentage = Math.Round(percentage, 2)
                };

                return Ok(new { success = true, data = report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/dashboard/fee-report
        [HttpGet("fee-report")]
        public async Task<ActionResult> GetFeeReport()
        {
            try
            {
                var totalAmount = await _context.Fees.SumAsync(f => f.Amount);
                var paidAmount = await _context.Fees.Where(f => f.Status == "Paid").SumAsync(f => f.Amount);
                var pendingAmount = await _context.Fees.Where(f => f.Status == "Pending").SumAsync(f => f.Amount);
                var overdueAmount = await _context.Fees.Where(f => f.Status == "Overdue").SumAsync(f => f.Amount);

                var paymentPercentage = totalAmount > 0 ? paidAmount / totalAmount * 100 : 0;

                var byClass = await _context.Fees
                    .Where(f => !string.IsNullOrEmpty(f.ClassId))
                    .GroupBy(f => f.ClassId)
                    .Select(g => new { ClassId = g.Key, Amount = g.Sum(x => x.Amount) })
                    .ToDictionaryAsync(x => x.ClassId ?? "Unknown", x => (int)x.Amount);

                var report = new
                {
                    totalAmount,
                    paidAmount,
                    pendingAmount,
                    overdueAmount,
                    paymentPercentage = Math.Round(paymentPercentage, 2),
                    byClass
                };

                return Ok(new { success = true, data = report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
        // GET: api/dashboard/revenue-data
        [HttpGet("revenue-data")]
        public async Task<ActionResult> GetRevenueData([FromQuery] int year = 0)
        {
            try
            {
                var targetYear = year == 0 ? DateTime.UtcNow.Year : year;

                // Get paid fees grouped by month
                var paidFees = await _context.Fees
                    .Where(f => f.Status == "Paid" && f.DueDate.StartsWith(targetYear.ToString()))
                    .ToListAsync();

                var revenueData = paidFees
                    .GroupBy(f => f.DueDate.Substring(0, 7)) // Group by YYYY-MM
                    .Select(g => new
                    {
                        month = g.Key,
                        revenue = (int)g.Sum(f => f.Amount)
                    })
                    .OrderBy(r => r.month)
                    .ToList();

                return Ok(new { success = true, data = revenueData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/dashboard/recent-activities
        [HttpGet("recent-activities")]
        public async Task<ActionResult> GetRecentActivities([FromQuery] int limit = 10)
        {
            try
            {
                var recentStudents = await _context.Students
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(limit)
                    .Select(s => new
                    {
                        type = "student",
                        action = "New student enrolled",
                        name = s.Name,
                        date = s.CreatedAt
                    })
                    .ToListAsync();

                var recentFees = await _context.Fees
                    .Where(f => f.Status == "Paid")
                    .OrderByDescending(f => f.UpdatedAt)
                    .Take(limit)
                    .Select(f => new
                    {
                        type = "fee",
                        action = "Fee paid",
                        name = f.StudentName,
                        date = f.UpdatedAt ?? f.CreatedAt
                    })
                    .ToListAsync();

                var activities = recentStudents
                    .Concat(recentFees)
                    .OrderByDescending(a => a.date)
                    .Take(limit)
                    .ToList();

                return Ok(new { success = true, data = activities });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}