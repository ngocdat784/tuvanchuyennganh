using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerOrientationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestsController(AppDbContext context)
        {
            _context = context;
        }

       [Authorize]
[HttpPost("start")]
public async Task<IActionResult> StartTest()
{
    var userIdClaim = User.FindFirst("userId");
    if (userIdClaim == null)
        return Unauthorized();

    int userId = int.Parse(userIdClaim.Value);

    var test = new TestResult
    {
        StudentId = userId, // hoặc map sang bảng Student
        SubmittedAt = DateTime.UtcNow
    };

    _context.TestResults.Add(test);
    await _context.SaveChangesAsync();

    return Ok(new { testResultId = test.TestResultId });
}



        // ================================
        // 2️⃣ Lưu câu trả lời + Impact
        // ================================
        public class SaveAnswerDTO
        {
            public int TestResultId { get; set; }
            public int QuestionId { get; set; }
            public int AnswerId { get; set; }
        }

        [HttpPost("answer")]
        public async Task<IActionResult> SaveAnswer([FromBody] SaveAnswerDTO dto)
        {
            // 🔥 1. Check TestResult tồn tại trước
            if (!await _context.TestResults.AnyAsync(x => x.TestResultId == dto.TestResultId))
                return BadRequest($"❌ TestResultId {dto.TestResultId} không tồn tại!");

            // 🔥 2. Check Question & Answer hợp lệ
            if (!await _context.Questions.AnyAsync(q => q.QuestionId == dto.QuestionId))
                return BadRequest("❌ QuestionId không hợp lệ");

            if (!await _context.Answers.AnyAsync(a => a.AnswerId == dto.AnswerId))
                return BadRequest("❌ AnswerId không hợp lệ");

            // 🔥 3. Tạo TestAnswer
            var testAnswer = new TestAnswer
            {
                TestResultId = dto.TestResultId,
                QuestionId = dto.QuestionId,
                AnswerId = dto.AnswerId
            };

            _context.TestAnswers.Add(testAnswer);
            await _context.SaveChangesAsync(); // cần để có TestAnswerId

            // 🔥 4. Load impact từ Answer
            var impacts = await _context.AnswerImpacts
                .Where(i => i.AnswerId == dto.AnswerId)
                .ToListAsync();

            foreach (var imp in impacts)
            {
                _context.TestAnswerImpacts.Add(new TestAnswerImpact
                {
                    TestAnswerId = testAnswer.TestAnswerId,
                    ImpactValue = imp.ImpactValue,
                    MajorId = imp.MajorId,
                    MajorTraitId = imp.MajorTraitId
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { saved = true, testAnswerId = testAnswer.TestAnswerId });
        }


        // ================================
        // 3️⃣ Hoàn thành test → tính điểm ngành
        // ================================
        [HttpPost("finish")]
        public async Task<IActionResult> FinishTest([FromBody] int testResultId)
        {
            var test = await _context.TestResults
                .Include(t => t.TestAnswers)
                .ThenInclude(ta => ta.Impacts)
                .FirstOrDefaultAsync(t => t.TestResultId == testResultId);

            if (test == null)
                return NotFound("❌ TestResult không tồn tại");

            var majorScores = test.TestAnswers
                .SelectMany(a => a.Impacts)
                .GroupBy(i => i.MajorId)
                .Select(g => new TestResultMajorScore
                {
                    TestResultId = testResultId,
                    MajorId = g.Key,
                    Score = g.Sum(i => i.ImpactValue)
                })
                .ToList();

            // 🔥 Xóa điểm cũ trước khi tạo điểm mới
            _context.TestResultMajorScores.RemoveRange(
                _context.TestResultMajorScores.Where(s => s.TestResultId == testResultId)
            );

            _context.TestResultMajorScores.AddRange(majorScores);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✔ Bài test đã hoàn thành", majorScores });
        }


        // ================================
        // 4️⃣ API lấy kết quả
        // ================================
        [HttpGet("result/{testResultId}")]
        public async Task<IActionResult> GetResult(int testResultId)
        {
            var scores = await _context.TestResultMajorScores
                .Where(s => s.TestResultId == testResultId)
                .Include(s => s.Major)
                .OrderByDescending(s => s.Score)
                .ToListAsync();

            return Ok(scores);
        }
    }
}
