using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.DTOs.Staff;

namespace SchoolManagement.API.Controllers.Staff
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffRepository _staffRepository;

        public StaffController(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        // GET: api/staff
        [HttpGet]
        public async Task<ActionResult> GetAllStaff(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? department = null,
            [FromQuery] string? role = null,
            [FromQuery] string? status = null)
        {
            try
            {
                IEnumerable<Core.Entities.Staff.Staff> staff;

                if (!string.IsNullOrEmpty(department))
                {
                    staff = await _staffRepository.GetByDepartmentAsync(department);
                }
                else if (!string.IsNullOrEmpty(role))
                {
                    staff = await _staffRepository.GetByRoleAsync(role);
                }
                else if (!string.IsNullOrEmpty(status))
                {
                    staff = await _staffRepository.GetByStatusAsync(status);
                }
                else
                {
                    staff = await _staffRepository.GetPagedAsync(page, limit);
                }

                var total = await _staffRepository.GetTotalCountAsync();

                return Ok(new
                {
                    success = true,
                    data = staff,
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
                return StatusCode(500, new { success = false, error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // GET: api/staff/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetStaff(int id)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    return NotFound(new { success = false, error = "Staff member not found" });
                }
                return Ok(new { success = true, data = staff });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/staff/staffid/STF001
        [HttpGet("staffid/{staffId}")]
        public async Task<ActionResult> GetStaffByStaffId(string staffId)
        {
            try
            {
                var staff = await _staffRepository.GetByStaffIdAsync(staffId);
                if (staff == null)
                {
                    return NotFound(new { success = false, error = "Staff member not found" });
                }
                return Ok(new { success = true, data = staff });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/staff/department/IT
        [HttpGet("department/{department}")]
        public async Task<ActionResult> GetStaffByDepartment(string department)
        {
            try
            {
                var staff = await _staffRepository.GetByDepartmentAsync(department);
                return Ok(new { success = true, data = staff });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // GET: api/staff/role/Librarian
        [HttpGet("role/{role}")]
        public async Task<ActionResult> GetStaffByRole(string role)
        {
            try
            {
                var staff = await _staffRepository.GetByRoleAsync(role);
                return Ok(new { success = true, data = staff });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // POST: api/staff
        [HttpPost]
        public async Task<ActionResult> CreateStaff([FromBody] CreateStaffRequest request)
        {
            try
            {
                var existingStaff = await _staffRepository.GetByStaffIdAsync(request.StaffId);
                if (existingStaff != null)
                {
                    return BadRequest(new { success = false, error = "Staff ID already exists" });
                }

                var existingEmail = await _staffRepository.GetByEmailAsync(request.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { success = false, error = "Email already exists" });
                }

                var staff = new Core.Entities.Staff.Staff
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    Dob = request.Dob,
                    StaffId = request.StaffId,
                    Role = request.Role,
                    Department = request.Department,
                    Gender = request.Gender,
                    Photo = request.Photo,
                    Status = request.Status,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdStaff = await _staffRepository.AddAsync(staff);

                return Ok(new { success = true, data = createdStaff, message = "Staff member created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // PUT: api/staff/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] CreateStaffRequest request)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    return NotFound(new { success = false, error = "Staff member not found" });
                }

                staff.Name = request.Name;
                staff.Email = request.Email;
                staff.Phone = request.Phone;
                staff.Dob = request.Dob;
                staff.Role = request.Role;
                staff.Department = request.Department;
                staff.Gender = request.Gender;
                staff.Photo = request.Photo;
                staff.Status = request.Status;
                staff.UpdatedAt = DateTime.UtcNow;

                await _staffRepository.UpdateAsync(staff);

                return Ok(new { success = true, data = staff, message = "Staff member updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // DELETE: api/staff/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    return NotFound(new { success = false, error = "Staff member not found" });
                }

                await _staffRepository.DeleteAsync(id);

                return Ok(new { success = true, message = "Staff member deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}