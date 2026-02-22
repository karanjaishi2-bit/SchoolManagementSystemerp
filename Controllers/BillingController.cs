using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SchoolManagement.Controllers
{
    public class BillingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly List<SelectListItem> _billingTypeOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "Annual", Text = "Annual" },
            new SelectListItem { Value = "Seasonal", Text = "Seasonal" },
            new SelectListItem { Value = "Monthly", Text = "Monthly" },
            new SelectListItem { Value = "Weekly", Text = "Weekly" }
        };

        public BillingController(ApplicationDbContext context) => _context = context;

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var bills = await _context.BillingMasters
                .Include(b => b.Student)
                .Include(b => b.BillingItems)
                    .ThenInclude(bi => bi.Course)
                .ToListAsync();

            return View(bills);
        }

        // ================= CREATE GET =================
        public IActionResult Create()
        {
            var model = new BillingViewModel
            {
                Students = _context.Students
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToList(),
                Courses = _context.Courses
                    .Select(c => new CourseItem { Id = c.Id, Name = c.Name, Fee = c.Fee })
                    .ToList(),
                BillingTypes = _billingTypeOptions
            };

            return View(model);
        }

        // ================= CREATE POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateBillingDropdowns(model);
                return View(model);
            }

            var bill = new BillingMaster
            {
                StudentId = model.StudentId,
                BillDate = System.DateTime.Now,
                BillingType = model.BillingType
            };
            _context.BillingMasters.Add(bill);
            await _context.SaveChangesAsync();

            if (model.SelectedCourseIds != null && model.SelectedCourseIds.Any())
            {
                foreach (var courseId in model.SelectedCourseIds)
                {
                    var course = await _context.Courses.FindAsync(courseId);
                    if (course != null)
                    {
                        _context.BillingItems.Add(new BillingItem
                        {
                            BillingMasterId = bill.Id,
                            CourseId = course.Id,
                            Amount = course.Fee
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT GET =================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bill = await _context.BillingMasters
                .Include(b => b.BillingItems)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null) return NotFound();

            var model = new BillingViewModel
            {
                Id = bill.Id,
                StudentId = bill.StudentId,
                BillingType = bill.BillingType,
                SelectedCourseIds = bill.BillingItems.Select(bi => bi.CourseId).ToList(),
                Students = _context.Students
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                    .ToList(),
                Courses = _context.Courses
                    .Select(c => new CourseItem { Id = c.Id, Name = c.Name, Fee = c.Fee })
                    .ToList(),
                BillingTypes = _billingTypeOptions
            };

            return View(model);
        }

        // ================= EDIT POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BillingViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateBillingDropdowns(model);
                return View(model);
            }

            var bill = await _context.BillingMasters
                .Include(b => b.BillingItems)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null) return NotFound();

            bill.StudentId = model.StudentId;
            bill.BillingType = model.BillingType;

            // Remove old items
            _context.BillingItems.RemoveRange(bill.BillingItems);

            // Add new selected courses
            if (model.SelectedCourseIds != null && model.SelectedCourseIds.Any())
            {
                foreach (var courseId in model.SelectedCourseIds)
                {
                    var course = await _context.Courses.FindAsync(courseId);
                    if (course != null)
                    {
                        _context.BillingItems.Add(new BillingItem
                        {
                            BillingMasterId = bill.Id,
                            CourseId = course.Id,
                            Amount = course.Fee
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bill = await _context.BillingMasters
                .Include(b => b.Student)
                .Include(b => b.BillingItems)
                    .ThenInclude(bi => bi.Course)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null) return NotFound();
            return View(bill);
        }

        // ================= PRINT =================
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null) return NotFound();

            var bill = await _context.BillingMasters
                .Include(b => b.Student)
                .Include(b => b.BillingItems)
                    .ThenInclude(bi => bi.Course)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null) return NotFound();
            return View(bill);
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bill = await _context.BillingMasters
                .Include(b => b.Student)
                .Include(b => b.BillingItems)
                    .ThenInclude(bi => bi.Course)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null) return NotFound();
            return View(bill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.BillingMasters
                .Include(b => b.BillingItems)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill != null)
            {
                _context.BillingItems.RemoveRange(bill.BillingItems);
                _context.BillingMasters.Remove(bill);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= HELPER =================
        private async Task PopulateBillingDropdowns(BillingViewModel model)
        {
            model.Students = _context.Students
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            model.Courses = _context.Courses
                .Select(c => new CourseItem { Id = c.Id, Name = c.Name, Fee = c.Fee })
                .ToList();

            model.BillingTypes = _billingTypeOptions;

            await Task.CompletedTask;
        }
    }
}
