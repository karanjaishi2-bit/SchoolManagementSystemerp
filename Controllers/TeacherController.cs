using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TeacherController(ApplicationDbContext context) => _context = context;

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }

            return View(await _context.Teachers.ToListAsync());
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // ================= DETAILS / EDIT =================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, Teacher teacher)
        {
            if (id != teacher.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(teacher);
        }

        // ================= DELETE =================
        // ================= DELETE =================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .Include(t => t.Courses) // Include linked courses
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound();

            // Pass assigned courses to the view
            ViewBag.AssignedCourses = teacher.Courses.Select(c => c.Name).ToList();

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Courses) // Include linked courses
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound();

            if (teacher.Courses.Any())
            {
                // Prevent deletion if courses exist
                TempData["Error"] = "Warning: This teacher is assigned to one or more courses. Reassign or delete these courses first.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
