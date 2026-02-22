using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using SchoolManagement.Data; // Add this for ApplicationDbContext
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get total students for dashboard
            var totalStudents = await _context.Students.CountAsync();
            ViewBag.TotalStudents = totalStudents;

            // You can also add other totals: Courses, Teachers, etc.
            // Example: ViewBag.TotalCourses = await _context.Courses.CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
