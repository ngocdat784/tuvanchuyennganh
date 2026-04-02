using CareerOrientationAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CareerOrientationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 📌 1. Lấy danh sách thông báo theo Student
        // ==========================================
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetNotifications(int studentId)
        {
            var list = await _context.Notifications
                .Where(n => n.StudentId == studentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(list);
        }

        // ==========================================
        // 📌 2. Tạo thông báo mới
        // ==========================================
      [HttpPost]
public async Task<IActionResult> CreateNotification([FromBody] Notification model)
{
    if (model == null)
        return BadRequest("Dữ liệu không hợp lệ");

    // Kiểm tra studentId có tồn tại
    var exists = await _context.Students.AnyAsync(s => s.StudentId == model.StudentId);
    if (!exists)
        return BadRequest("Student không tồn tại");

    // Server tự set thời gian
    model.CreatedAt = DateTime.Now;
    model.IsRead = false;

    _context.Notifications.Add(model);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Notification created", model });
}


        [HttpPut("mark-read/{studentId}")]
public async Task<IActionResult> MarkAllAsRead(int studentId)
{
    // Lấy toàn bộ thông báo chưa đọc
    var notifications = await _context.Notifications
        .Where(n => n.StudentId == studentId && n.IsRead == false)
        .ToListAsync();

    if (!notifications.Any())
        return Ok("Không có thông báo chưa đọc");

    // Đánh dấu tất cả là đã đọc
    foreach (var n in notifications)
    {
        n.IsRead = true;
    }

    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Tất cả thông báo đã được đánh dấu là đã đọc",
        count = notifications.Count
    });
}


        // ==========================================
        // 📌 4. Xoá 1 thông báo
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var n = await _context.Notifications.FindAsync(id);
            if (n == null)
                return NotFound();

            _context.Notifications.Remove(n);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thành công" });
        }

        // ==========================================
        // 📌 5. Xoá tất cả thông báo của 1 student
        // ==========================================
        [HttpDelete("clear/{studentId}")]
        public async Task<IActionResult> ClearAll(int studentId)
        {
            var list = await _context.Notifications
                .Where(n => n.StudentId == studentId)
                .ToListAsync();

            _context.Notifications.RemoveRange(list);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa toàn bộ thông báo" });
        }
    }
}
