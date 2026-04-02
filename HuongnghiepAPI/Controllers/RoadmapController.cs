using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadmapController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoadmapController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 🔥 1) LẤY LỘ TRÌNH THEO NGÀNH
        // GET: api/roadmap/major/5
        // ============================================================
        [HttpGet("major/{majorId}")]
        public async Task<IActionResult> GetRoadmapByMajor(int majorId)
        {
            var roadmap = await _context.Roadmaps
                .Where(r => r.MajorId == majorId)
                .Include(r => r.Major)
                .Include(r => r.Stages)
                    .ThenInclude(s => s.Items)
                        .ThenInclude(i => i.Skills)
                .FirstOrDefaultAsync();

            if (roadmap == null)
                return NotFound(new { message = "Ngành này chưa có lộ trình." });

            return Ok(roadmap);
        }


        // ============================================================
        // 2) GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roadmaps = await _context.Roadmaps
                .Include(r => r.Major)
                .Include(r => r.Stages)
                    .ThenInclude(s => s.Items)
                        .ThenInclude(i => i.Skills)
                .ToListAsync();

            return Ok(roadmaps);
        }


        // ============================================================
        // 3) GET BY ID
        // ============================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var roadmap = await _context.Roadmaps
                .Include(r => r.Major)
                .Include(r => r.Stages)
                    .ThenInclude(s => s.Items)
                        .ThenInclude(i => i.Skills)
                .FirstOrDefaultAsync(r => r.RoadmapId == id);

            if (roadmap == null)
                return NotFound();

            return Ok(roadmap);
        }


        // ============================================================
        // 4) CREATE ROADMAP
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Roadmap model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Roadmaps.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }


        // ============================================================
        // 5) UPDATE ROADMAP
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Roadmap model)
        {
            var roadmap = await _context.Roadmaps.FindAsync(id);
            if (roadmap == null)
                return NotFound();

            roadmap.Title = model.Title;
            roadmap.Description = model.Description;
            roadmap.MajorId = model.MajorId;

            await _context.SaveChangesAsync();
            return Ok(roadmap);
        }


        // ============================================================
        // 6) DELETE ROADMAP + STAGE + ITEM + SKILL
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var roadmap = await _context.Roadmaps
                .Include(r => r.Stages)
                    .ThenInclude(s => s.Items)
                        .ThenInclude(i => i.Skills)
                .FirstOrDefaultAsync(r => r.RoadmapId == id);

            if (roadmap == null)
                return NotFound();

            // Xóa skill
            foreach (var stage in roadmap.Stages)
                foreach (var item in stage.Items)
                    _context.RoadmapSkills.RemoveRange(item.Skills);

            // Xóa item
            foreach (var stage in roadmap.Stages)
                _context.RoadmapItems.RemoveRange(stage.Items);

            // Xóa stage
            _context.RoadmapStages.RemoveRange(roadmap.Stages);

            // Xóa roadmap
            _context.Roadmaps.Remove(roadmap);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
