using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entities.Teachers;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.DTOs.Teachers;
namespace SchoolManagement.API.Controllers.Teachers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeachersController(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        // GET: api/teachers
        [HttpGet]
        public async Task<ActionResult> GetAllTeachers(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? subject = null,
            [FromQuery] string? search = null)
        {
            try
            {
                IEnumerable<Teacher> teachers;

                if (!string.IsNullOrEmpty(search))
                {
                    teachers = (await _teacherRepository.GetAllAsync())
                        .Where(t => t.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                   t.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else if (!string.IsNullOrEmpty(subject))
                {
                    teachers = await _teacherRepository.GetBySubjectAsync(subject);
                }
                else if (!string.IsNullOrEmpty(status))
                {
                    teachers = await _teacherRepository.GetByStatusAsync(status);
                }
                else
                {
                    teachers = await _teacherRepository.GetPagedAsync(page, limit);
                }

                var total = await _teacherRepository.GetTotalCountAsync();

                return Ok(new
                {
                    success = true,
                    data = teachers,
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

        // GET: api/teachers/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTeacher(int id)
        {
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(id);
                if (teacher == null)
                {
                    return NotFound(new { success = false, error = "Teacher not found" });
                }
                return Ok(new { success = true, data = teacher });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // POST: api/teachers
        [HttpPost]
        public async Task<ActionResult> CreateTeacher([FromBody] CreateTeacherRequest request)
        {
            try
            {
                var existingTeacher = await _teacherRepository.GetByTeacherIdAsync(request.TeacherId);
                if (existingTeacher != null)
                {
                    return BadRequest(new { success = false, error = "Teacher ID already exists" });
                }

                var existingEmail = await _teacherRepository.GetByEmailAsync(request.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { success = false, error = "Email already exists" });
                }

                var teacher = new Teacher
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    Dob = request.Dob,
                    TeacherId = request.TeacherId,
                    Subject = request.Subject,
                    Status = request.Status,
                    Gender = request.Gender,
                    Photo = request.Photo,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdTeacher = await _teacherRepository.AddAsync(teacher);

                return Ok(new { success = true, data = createdTeacher, message = "Teacher created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // PUT: api/teachers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] CreateTeacherRequest request)
        {
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(id);
                if (teacher == null)
                {
                    return NotFound(new { success = false, error = "Teacher not found" });
                }

                teacher.Name = request.Name;
                teacher.Email = request.Email;
                teacher.Phone = request.Phone;
                teacher.Dob = request.Dob;
                teacher.Subject = request.Subject;
                teacher.Status = request.Status;
                teacher.Gender = request.Gender;
                teacher.Photo = request.Photo;
                teacher.UpdatedAt = DateTime.UtcNow;

                await _teacherRepository.UpdateAsync(teacher);

                return Ok(new { success = true, data = teacher, message = "Teacher updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // DELETE: api/teachers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(id);
                if (teacher == null)
                {
                    return NotFound(new { success = false, error = "Teacher not found" });
                }

                await _teacherRepository.DeleteAsync(id);

                return Ok(new { success = true, message = "Teacher deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}