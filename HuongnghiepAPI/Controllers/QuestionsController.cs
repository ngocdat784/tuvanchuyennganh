using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public QuestionsController(AppDbContext db)
        {
            _db = db;
        }

        // =========================================================
        // GET: api/question
        // Lấy tất cả câu hỏi (kèm Answers)
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _db.Questions
                .Include(q => q.Answers)
                .Include(q => q.QuestionGroup)
                .ToListAsync();

            return Ok(questions);
        }

        // =========================================================
        // GET: api/question/{id}
        // Lấy 1 câu hỏi theo ID
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _db.Questions
                .Include(q => q.Answers)
                .Include(q => q.QuestionGroup)
                .FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null)
                return NotFound(new { message = "Không tìm thấy câu hỏi." });

            return Ok(question);
        }

        // =========================================================
        // GET: api/question/group/{groupId}
        // Lấy tất cả câu hỏi thuộc một nhóm
        // =========================================================
        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetQuestionsByGroup(int groupId)
        {
            var questions = await _db.Questions
                .Where(q => q.QuestionGroupId == groupId)
                .Include(q => q.Answers)
                .OrderBy(q => q.Order)
                .ToListAsync();

            return Ok(questions);
        }

        // =========================================================
        // POST: api/question
        // Tạo mới câu hỏi
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.Questions.Add(question);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestionById), new { id = question.QuestionId }, question);
        }

        // =========================================================
        // PUT: api/question/{id}
        // Sửa câu hỏi
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] Question update)
        {
            var question = await _db.Questions.FindAsync(id);
            if (question == null)
                return NotFound(new { message = "Không tìm thấy câu hỏi." });

            question.Text = update.Text;
            question.Order = update.Order;
            question.QuestionGroupId = update.QuestionGroupId;

            _db.Questions.Update(question);
            await _db.SaveChangesAsync();

            return Ok(question);
        }

        // =========================================================
        // DELETE: api/question/{id}
        // Xoá câu hỏi
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _db.Questions.FindAsync(id);
            if (question == null)
                return NotFound(new { message = "Không tìm thấy câu hỏi." });

            _db.Questions.Remove(question);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Xoá câu hỏi thành công." });
        }
    }
}
