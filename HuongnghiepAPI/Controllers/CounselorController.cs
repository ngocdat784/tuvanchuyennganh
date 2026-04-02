using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CounselorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CounselorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ====================================
        // HELPER: HASH PASSWORD
        // ====================================
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }

        // ====================================
        // HELPER: SAVE IMAGE
        // ====================================
        private string SaveImage(IFormFile file)
        {
            string uploadPath = Path.Combine(_env.WebRootPath, "images", "counselors");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string ext = Path.GetExtension(file.FileName);
            string fileName = Guid.NewGuid().ToString() + ext;

            string fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"/images/counselors/{fileName}";
        }

        // ====================================
        // HELPER: DELETE IMAGE
        // ====================================
        private void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string fullPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }

        // ====================================
        // 1. Get all counselors
        // ====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Counselors.ToListAsync();
            return Ok(list);
        }

        // ====================================
        // 2. Get counselor by ID
        // ====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var counselor = await _context.Counselors.FindAsync(id);
            if (counselor == null)
                return NotFound("Counselor not found");

            return Ok(counselor);
        }

        // ====================================
// 3️⃣ Register Counselor (Admin only)
// ====================================
[Authorize(Roles = "Admin")]
[HttpPost("register")]
public async Task<IActionResult> Register(
    [FromForm] string fullName,
    [FromForm] string email,
    [FromForm] string password,
    [FromForm] string? details,
    [FromForm] IFormFile? image)
{
    if (await _context.Counselors.AnyAsync(x => x.Email == email))
        return BadRequest("Email already exists");

    var counselor = new Counselor
    {
        FullName = fullName,
        Email = email,
        PasswordHash = HashPassword(password),
        Details = details,
        Role = "counselor" // ✅ GÁN ROLE RÕ RÀNG
    };

    // Upload image
    if (image != null)
        counselor.ImageUrl = SaveImage(image);

    _context.Counselors.Add(counselor);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Tạo Counselor thành công",
        counselorId = counselor.CounselorId
    });
}
        [Authorize(Roles = "Admin,counselor")]
[HttpPost("{id}/upload-image")]
public async Task<IActionResult> UploadImage(int id, IFormFile file)
{
    var counselor = await _context.Counselors.FindAsync(id);
    if (counselor == null)
        return NotFound("Counselor not found");

    // 🔐 LẤY THÔNG TIN TỪ TOKEN
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    var userId = int.Parse(User.FindFirst("userId")!.Value);

    // ❌ Counselor không được sửa ảnh của người khác
    if (role == "counselor" && counselor.CounselorId != userId)
        return Forbid("Bạn không có quyền sửa ảnh của Counselor khác");

    // Delete old image
    DeleteImage(counselor.ImageUrl);

    // Save new image
    counselor.ImageUrl = SaveImage(file);

    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Cập nhật ảnh thành công",
        imageUrl = counselor.ImageUrl
    });
}

        // ====================================
        // 5. Login
        // ====================================
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hash = HashPassword(password);

            var counselor = await _context.Counselors
                .FirstOrDefaultAsync(c => c.Email == email && c.PasswordHash == hash);

            if (counselor == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                message = "Login successful",
                counselorId = counselor.CounselorId,
                name = counselor.FullName,
                image = counselor.ImageUrl
            });
        }

      [Authorize(Roles = "Admin,counselor")]
[HttpGet("{id}/schedules")]
public async Task<IActionResult> GetSchedules(int id)
{
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    var userId = int.Parse(User.FindFirst("userId")!.Value);

    // ❌ Counselor không được xem lịch của counselor khác
    if (role == "counselor" && userId != id)
        return Forbid("Bạn không có quyền xem lịch của tư vấn viên khác");

    var schedules = await _context.CounselingSchedules
        .Where(s => s.CounselorId == id)
        .OrderByDescending(s => s.BookingTime)
        .ToListAsync();

    return Ok(schedules);
}


        // ====================================
        // 7. Create schedule
        // ====================================
        [HttpPost("schedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] CounselingSchedule model)
        {
            if (!await _context.Counselors.AnyAsync(c => c.CounselorId == model.CounselorId))
                return BadRequest("Counselor not found");

            _context.CounselingSchedules.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Schedule created", model.ScheduleId });
        }

        // ====================================
        // 8. Update schedule status
        // ====================================
        [HttpPut("schedule/{id}/status")]
public async Task<IActionResult> UpdateStatus(int id, string status)
{
    var schedule = await _context.CounselingSchedules.FindAsync(id);
    if (schedule == null)
        return NotFound("Schedule not found");

    schedule.Status = status;
    await _context.SaveChangesAsync();

    // 🔔 Tạo thông báo cho học sinh
    string msg = status switch
    {
        "Approved" => "📢 Lịch tư vấn của bạn đã được chấp nhận!",
        "Rejected" => "⚠️ Lịch tư vấn của bạn đã bị từ chối.",
        "Completed" => "✔️ Buổi tư vấn của bạn đã hoàn tất.",
        _ => "🔔 Lịch tư vấn có cập nhật mới."
    };

    _context.Notifications.Add(new Notification
    {
        StudentId = schedule.StudentId,
        Message = msg
    });

    await _context.SaveChangesAsync();

    return Ok(new { message = "Status updated" });
}
[HttpGet("/api/schedule")]
public async Task<IActionResult> GetAllSchedules()
{
    var list = await _context.CounselingSchedules
        .OrderByDescending(s => s.BookingTime)
        .ToListAsync();

    return Ok(list);
}

        
        

        // ====================================
        // 9. Delete schedule
        // ====================================
        [HttpDelete("schedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.CounselingSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound("Schedule not found");

            _context.CounselingSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Schedule deleted" });
        }

        // ====================================
        // 10. Delete counselor (delete image too)
        // ====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCounselor(int id)
        {
            var counselor = await _context.Counselors.FindAsync(id);
            if (counselor == null)
                return NotFound("Counselor not found");

            // delete image file
            DeleteImage(counselor.ImageUrl);

            _context.Counselors.Remove(counselor);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Counselor deleted" });
        }
        // ====================================
// 11. Update counselor info
// ====================================
[Authorize(Roles = "Admin,counselor")]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateCounselor(
    int id,
    [FromBody] Counselor updated)
{
    var counselor = await _context.Counselors.FindAsync(id);
    if (counselor == null)
        return NotFound("Counselor not found");

    // 🔐 LẤY ROLE & USER ID TỪ TOKEN
    var role = User.FindFirst(ClaimTypes.Role)?.Value;
    var userId = int.Parse(User.FindFirst("userId")!.Value);

    // ❌ Counselor chỉ được sửa chính mình
    if (role == "counselor" && counselor.CounselorId != userId)
        return Forbid("Bạn không có quyền sửa tư vấn viên khác");

    // ❗ Không cho sửa email trùng
    if (counselor.Email != updated.Email &&
        await _context.Counselors.AnyAsync(c => c.Email == updated.Email))
    {
        return BadRequest("Email đã tồn tại");
    }

    // ✅ CẬP NHẬT CÁC FIELD CHO PHÉP
    counselor.FullName = updated.FullName;
    counselor.Email = updated.Email;
    counselor.Details = updated.Details;

    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Cập nhật tư vấn viên thành công"
    });
}

    }
}
