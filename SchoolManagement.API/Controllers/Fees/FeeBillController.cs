using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.DTOs.Fees;
using SchoolManagement.Core.Entities.Fees;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.API.Controllers.Fees
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeeBillController : ControllerBase
    {
        private readonly IFeeBillRepository _feeBillRepository;
        private readonly IFeeRepository _feeRepository;
        private readonly ApplicationDbContext _context;

        public FeeBillController(
            IFeeBillRepository feeBillRepository,
            IFeeRepository feeRepository,
            ApplicationDbContext context)
        {
            _feeBillRepository = feeBillRepository;
            _feeRepository = feeRepository;
            _context = context;
        }

        // GET: api/feebill
        [HttpGet]
        public async Task<ActionResult> GetAllFeeBills(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] int? studentId = null,
            [FromQuery] string? classId = null,
            [FromQuery] string? status = null)
        {
            try
            {
                IEnumerable<FeeBill> feeBills;

                if (studentId.HasValue)
                {
                    feeBills = await _feeBillRepository.GetByStudentIdAsync(studentId.Value);
                }
                else if (!string.IsNullOrEmpty(classId))
                {
                    feeBills = await _feeBillRepository.GetByClassIdAsync(classId);
                }
                else if (!string.IsNullOrEmpty(status))
                {
                    feeBills = await _feeBillRepository.GetByStatusAsync(status);
                }
                else
                {
                    feeBills = await _feeBillRepository.GetPagedAsync(page, limit);
                }

                var total = await _feeBillRepository.GetTotalCountAsync();

                return Ok(new
                {
                    success = true,
                    data = feeBills,
                    pagination = new
                    {
                        page,
                        limit,
                        total,
                        pages = (int)Math.Ceiling(total / (double)limit)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/feebill/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetFeeBill(int id)
        {
            try
            {
                var feeBill = await _context.FeeBills
                    .Include(f => f.FeeItems)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (feeBill == null)
                    return NotFound(new { success = false, error = "Fee bill not found" });

                return Ok(new { success = true, data = feeBill });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/feebill/student/5
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult> GetFeeBillsByStudent(int studentId)
        {
            try
            {
                var feeBills = await _feeBillRepository.GetByStudentIdAsync(studentId);
                return Ok(new { success = true, data = feeBills });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/feebill/class/10A
        [HttpGet("class/{classId}")]
        public async Task<ActionResult> GetFeeBillsByClass(string classId)
        {
            try
            {
                var feeBills = await _feeBillRepository.GetByClassIdAsync(classId);
                return Ok(new { success = true, data = feeBills });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/feebill/overdue
        [HttpGet("overdue")]
        public async Task<ActionResult> GetOverdueFeeBills()
        {
            try
            {
                var feeBills = await _feeBillRepository.GetOverdueBillsAsync();
                return Ok(new { success = true, data = feeBills });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // POST: api/feebill
        [HttpPost]
        public async Task<ActionResult> CreateFeeBill([FromBody] CreateFeeBillRequest request)
        {
            try
            {
                var feeBill = new FeeBill
                {
                    StudentId     = request.StudentId,
                    StudentName   = request.StudentName,
                    ClassId       = request.ClassId ?? string.Empty,
                    ClassName     = request.ClassName ?? string.Empty,
                    BillDate      = request.BillDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    DueDate       = request.DueDate,
                    TotalAmount   = request.TotalAmount,
                    PaidAmount    = request.PaidAmount,
                    BalanceAmount = request.BalanceAmount,
                    Status        = request.Status ?? "Pending",
                    CreatedAt     = DateTime.UtcNow,
                    IsActive      = true
                };

                var created = await _feeBillRepository.AddAsync(feeBill);

                // Save FeeItems linked to this FeeBill
                if (request.FeeItems != null && request.FeeItems.Any())
                {
                    foreach (var item in request.FeeItems)
                    {
                        _context.FeeItems.Add(new FeeItem
                        {
                            FeeHead     = item.FeeHead,
                            Amount      = item.Amount,
                            FeeType     = item.FeeType,
                            Frequency   = item.Frequency,
                            Description = item.Description,
                            FeeBillId   = created.Id,
                            CreatedAt   = DateTime.UtcNow,
                            IsActive    = true
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                // Return the bill with its items
                var result = await _context.FeeBills
                    .Include(f => f.FeeItems)
                    .FirstOrDefaultAsync(f => f.Id == created.Id);

                return Ok(new { success = true, data = result, message = "Fee bill created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // PUT: api/feebill/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeeBill(int id, [FromBody] CreateFeeBillRequest request)
        {
            try
            {
                var feeBill = await _context.FeeBills
                    .Include(f => f.FeeItems)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (feeBill == null)
                    return NotFound(new { success = false, error = "Fee bill not found" });

                feeBill.StudentId     = request.StudentId;
                feeBill.StudentName   = request.StudentName;
                feeBill.ClassId       = request.ClassId ?? feeBill.ClassId;
                feeBill.ClassName     = request.ClassName ?? feeBill.ClassName;
                feeBill.BillDate      = request.BillDate ?? feeBill.BillDate;
                feeBill.DueDate       = request.DueDate;
                feeBill.TotalAmount   = request.TotalAmount;
                feeBill.PaidAmount    = request.PaidAmount;
                feeBill.BalanceAmount = request.BalanceAmount;
                feeBill.Status        = request.Status ?? feeBill.Status;
                feeBill.UpdatedAt     = DateTime.UtcNow;

                // Replace FeeItems if provided
                if (request.FeeItems != null)
                {
                    // Remove old items
                    var oldItems = _context.FeeItems.Where(i => i.FeeBillId == id);
                    _context.FeeItems.RemoveRange(oldItems);

                    // Add new items
                    foreach (var item in request.FeeItems)
                    {
                        _context.FeeItems.Add(new FeeItem
                        {
                            FeeHead     = item.FeeHead,
                            Amount      = item.Amount,
                            FeeType     = item.FeeType,
                            Frequency   = item.Frequency,
                            Description = item.Description,
                            FeeBillId   = id,
                            CreatedAt   = DateTime.UtcNow,
                            IsActive    = true
                        });
                    }
                }

                await _context.SaveChangesAsync();

                var result = await _context.FeeBills
                    .Include(f => f.FeeItems)
                    .FirstOrDefaultAsync(f => f.Id == id);

                return Ok(new { success = true, data = result, message = "Fee bill updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // PATCH: api/feebill/5/pay
        [HttpPatch("{id}/pay")]
        public async Task<IActionResult> RecordPayment(int id, [FromBody] PaymentRequest request)
        {
            try
            {
                var feeBill = await _feeBillRepository.GetByIdAsync(id);
                if (feeBill == null)
                    return NotFound(new { success = false, error = "Fee bill not found" });

                feeBill.PaidAmount    += request.Amount;
                feeBill.BalanceAmount  = feeBill.TotalAmount - feeBill.PaidAmount;

                if (feeBill.BalanceAmount <= 0)
                    feeBill.Status = "Paid";
                else if (feeBill.PaidAmount > 0)
                    feeBill.Status = "Partial";

                feeBill.UpdatedAt = DateTime.UtcNow;

                await _feeBillRepository.UpdateAsync(feeBill);

                return Ok(new { success = true, data = feeBill, message = "Payment recorded successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // DELETE: api/feebill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeeBill(int id)
        {
            try
            {
                var feeBill = await _feeBillRepository.GetByIdAsync(id);
                if (feeBill == null)
                    return NotFound(new { success = false, error = "Fee bill not found" });

                // Delete associated FeeItems first
                var items = _context.FeeItems.Where(i => i.FeeBillId == id);
                _context.FeeItems.RemoveRange(items);
                await _context.SaveChangesAsync();

                await _feeBillRepository.DeleteAsync(id);

                return Ok(new { success = true, message = "Fee bill deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}