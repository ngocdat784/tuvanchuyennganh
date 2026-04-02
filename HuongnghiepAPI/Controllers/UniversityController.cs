using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Models;
using CareerOrientationAPI.Data;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UniversityController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. Lấy tất cả trường
        // GET: api/university
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var universities = await _context.Universities
                .Include(u => u.UniversityMajors)
                    .ThenInclude(um => um.Major)
                .AsNoTracking()
                .ToListAsync();

            return Ok(universities);
        }

        // ==========================================
        // 2. Lấy chi tiết 1 trường
        // GET: api/university/{id}
        // ==========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var university = await _context.Universities
                .Include(u => u.UniversityMajors)
                    .ThenInclude(um => um.Major)
                .FirstOrDefaultAsync(u => u.UniversityId == id);

            if (university == null)
                return NotFound("Không tìm thấy trường");

            return Ok(university);
        }

        // ==========================================
        // 3. GỢI Ý TRƯỜNG THEO NGÀNH
        // GET: api/university/by-major/{majorId}
        // ==========================================
        [HttpGet("by-major/{majorId}")]
        public async Task<IActionResult> GetByMajor(int majorId)
        {
            var universities = await _context.UniversityMajors
                .Where(um => um.MajorId == majorId)
                .Include(um => um.University)
                .Include(um => um.Major)
                .OrderByDescending(um => um.Priority)
                .Select(um => um.University)
                .Distinct()
                .ToListAsync();

            if (!universities.Any())
                return NotFound("Không có trường phù hợp với ngành này");

            return Ok(universities);
        }

        // ==========================================
        // 4. Thêm trường mới
        // POST: api/university
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Create(University university)
        {
            _context.Universities.Add(university);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = university.UniversityId },
                university);
        }

        // ==========================================
        // 5. Cập nhật trường
        // PUT: api/university/{id}
        // ==========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, University university)
        {
            if (id != university.UniversityId)
                return BadRequest("Id không khớp");

            _context.Entry(university).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ==========================================
        // 6. Xoá trường
        // DELETE: api/university/{id}
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var university = await _context.Universities.FindAsync(id);
            if (university == null)
                return NotFound();

            _context.Universities.Remove(university);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
