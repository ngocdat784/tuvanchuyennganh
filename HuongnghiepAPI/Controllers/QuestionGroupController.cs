using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionGroupController : ControllerBase
    {
        private readonly AppDbContext _db;

        public QuestionGroupController(AppDbContext db)
        {
            _db = db;
        }

        // =========================================================
        // GET: api/questiongroup
        // Lấy tất cả nhóm câu hỏi
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _db.QuestionGroups
                .OrderBy(g => g.Order)
                .ToListAsync(); // ❗ Không Include Questions để tránh vòng lặp JSON

            return Ok(groups);
        }

        // =========================================================
        // GET: api/questiongroup/{id}
        // Lấy 1 nhóm theo ID (kèm danh sách câu hỏi)
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var group = await _db.QuestionGroups
                .Include(g => g.Questions)
                .FirstOrDefaultAsync(g => g.QuestionGroupId == id);

            if (group == null)
                return NotFound(new { message = "Không tìm thấy nhóm câu hỏi." });

            return Ok(group);
        }

        // =========================================================
        // POST: api/questiongroup
        // Tạo mới nhóm câu hỏi
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] QuestionGroup group)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.QuestionGroups.Add(group);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroupById), new { id = group.QuestionGroupId }, group);
        }

        // =========================================================
        // PUT: api/questiongroup/{id}
        // Cập nhật nhóm câu hỏi
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] QuestionGroup update)
        {
            var group = await _db.QuestionGroups.FindAsync(id);

            if (group == null)
                return NotFound(new { message = "Không tìm thấy nhóm câu hỏi." });

            group.Title = update.Title;
            group.Description = update.Description;
            group.Order = update.Order;

            _db.QuestionGroups.Update(group);
            await _db.SaveChangesAsync();

            return Ok(group);
        }

        // =========================================================
        // DELETE: api/questiongroup/{id}
        // Xóa nhóm câu hỏi
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _db.QuestionGroups.FindAsync(id);

            if (group == null)
                return NotFound(new { message = "Không tìm thấy nhóm câu hỏi." });

            _db.QuestionGroups.Remove(group);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công nhóm câu hỏi." });
        }
    }
}
