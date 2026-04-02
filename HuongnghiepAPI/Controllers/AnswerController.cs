using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnswersController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1️⃣ LẤY TẤT CẢ ĐÁP ÁN
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAll()
        {
            return await _context.Answers
                                 .Include(a => a.Impacts)
                                 .ToListAsync();
        }

        // ============================================================
        // 2️⃣ LẤY ĐÁP ÁN THEO ID
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Answer>> GetById(int id)
        {
            var ans = await _context.Answers
                                    .Include(a => a.Impacts)
                                    .FirstOrDefaultAsync(a => a.AnswerId == id);

            if (ans == null)
                return NotFound();

            return ans;
        }

        // ============================================================
        // 3️⃣ LẤY ĐÁP ÁN THEO QUESTION ID
        // ============================================================
        [HttpGet("question/{questionId}")]
        public async Task<ActionResult<IEnumerable<Answer>>> GetByQuestion(int questionId)
        {
            return await _context.Answers
                                 .Where(a => a.QuestionId == questionId)
                                 .Include(a => a.Impacts)
                                 .OrderBy(a => a.Order)
                                 .ToListAsync();
        }

        // ============================================================
        // 4️⃣ TẠO ĐÁP ÁN
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<Answer>> Create(Answer model)
        {
            _context.Answers.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.AnswerId }, model);
        }

        // ============================================================
        // 5️⃣ CẬP NHẬT ĐÁP ÁN
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Answer model)
        {
            if (id != model.AnswerId)
                return BadRequest();

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Answers.Any(e => e.AnswerId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // ============================================================
        // 6️⃣ XÓA ĐÁP ÁN
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ans = await _context.Answers.FindAsync(id);
            if (ans == null)
                return NotFound();

            _context.Answers.Remove(ans);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // 7️⃣ THÊM IMPACT CHO ĐÁP ÁN
        // ============================================================
        [HttpPost("{answerId}/impact")]
        public async Task<ActionResult<AnswerImpact>> AddImpact(int answerId, AnswerImpact impact)
        {
            if (answerId != impact.AnswerId)
                return BadRequest("AnswerId không trùng khớp.");

            _context.AnswerImpacts.Add(impact);
            await _context.SaveChangesAsync();

            return impact;
        }

        // ============================================================
        // 8️⃣ XÓA IMPACT
        // ============================================================
        [HttpDelete("impact/{impactId}")]
        public async Task<IActionResult> DeleteImpact(int impactId)
        {
            var impact = await _context.AnswerImpacts.FindAsync(impactId);

            if (impact == null)
                return NotFound();

            _context.AnswerImpacts.Remove(impact);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
