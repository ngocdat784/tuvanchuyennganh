using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MajorController(AppDbContext db)
        {
            _db = db;
        }

        // ===========================
// GET: api/major
// ===========================
[HttpGet]
public async Task<IActionResult> GetAllMajors()
{
    var majors = await _db.Majors
        .Include(m => m.MajorTraits)
        .Include(m => m.TestResultScores)
        .Include(m => m.AnswerImpacts)
        .ToListAsync();

    bool needSave = false;

    // 🔥 AUTO BACKFILL KEYWORDS CHO NGÀNH CŨ
    foreach (var major in majors)
    {
        if (string.IsNullOrWhiteSpace(major.Keywords))
        {
            major.Keywords = GenerateKeywords(major.Name);
            needSave = true;
        }
    }

    if (needSave)
        await _db.SaveChangesAsync();

    return Ok(majors);
}


        // ===========================
        // GET: api/major/{id}
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMajorById(int id)
        {
            var major = await _db.Majors
                .Include(m => m.MajorTraits)
                .Include(m => m.AnswerImpacts)
                .Include(m => m.TestResultScores)
                .FirstOrDefaultAsync(m => m.MajorId == id);

            if (major == null)
                return NotFound(new { message = "Major không tồn tại." });

            return Ok(major);
        }

       [HttpPost]
public async Task<IActionResult> CreateMajor([FromBody] Major major)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // 🔥 AUTO GENERATE KEYWORDS
    major.Keywords = GenerateKeywords(major.Name);

    _db.Majors.Add(major);
    await _db.SaveChangesAsync();

    return CreatedAtAction(nameof(GetMajorById),
        new { id = major.MajorId }, major);
}


        [HttpPut("{id}")]
public async Task<IActionResult> UpdateMajor(int id, [FromBody] Major update)
{
    var major = await _db.Majors.FindAsync(id);
    if (major == null)
        return NotFound(new { message = "Major không tồn tại." });

    // 🔎 CHECK ĐỔI TÊN NGÀNH
    bool isNameChanged = 
        !string.Equals(major.Name?.Trim(), update.Name?.Trim(), StringComparison.OrdinalIgnoreCase);

    // ===== THÔNG TIN CƠ BẢN =====
    major.Name = update.Name;
    major.Description = update.Description;
    major.Details = update.Details;

    // ===== 🔥 THÔNG TIN CHI TIẾT NGÀNH =====
    major.Difficulty = update.Difficulty;
    major.Career = update.Career;

    // ===== 🔑 AUTO UPDATE KEYWORDS KHI ĐỔI TÊN =====
    if (isNameChanged)
    {
        major.Keywords = GenerateKeywords(major.Name);
    }

    _db.Majors.Update(major);
    await _db.SaveChangesAsync();

    return Ok(major);
}



        // ===========================
        // DELETE: api/major/{id}
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMajor(int id)
        {
            var major = await _db.Majors.FindAsync(id);
            if (major == null)
                return NotFound(new { message = "Major không tồn tại." });

            _db.Majors.Remove(major);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Xóa Major thành công." });
        }

        // ========================================================================
        // GET SUBFIELDS THEO MAJOR
        // ========================================================================
        [HttpGet("{id}/subfields")]
        public async Task<IActionResult> GetSubFields(int id)
        {
            var subfields = await _db.MajorSubFields
                .Where(sf => sf.MajorId == id)
                .ToListAsync();

            return Ok(subfields);
        }

        // ========================================================================
        // GET TRAITS THEO MAJOR
        // ========================================================================
        [HttpGet("{id}/traits")]
        public async Task<IActionResult> GetTraits(int id)
        {
            var traits = await _db.MajorTraits
                .Where(t => t.MajorId == id)
                .ToListAsync();

            return Ok(traits);
        }
        // ======================================================
// 🔤 SINH KEYWORDS TỰ ĐỘNG CHO NGÀNH
// ======================================================
private string GenerateKeywords(string majorName)
{
    if (string.IsNullOrWhiteSpace(majorName))
        return string.Empty;

    // Chuẩn hóa
    string normalized = NormalizeText(majorName);

    var keywords = new HashSet<string>();

    // 1️⃣ Tên đầy đủ
    keywords.Add(normalized);

    // 2️⃣ Tách từng từ
    var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    foreach (var w in words)
    {
        if (w.Length > 2)
            keywords.Add(w);
    }

    // 3️⃣ Viết tắt (CNTT, QTKD…)
    var abbreviation = string.Concat(words.Select(w => w[0]));
    if (abbreviation.Length > 1)
        keywords.Add(abbreviation);

    return string.Join(",", keywords);
}
private string NormalizeText(string text)
{
    if (string.IsNullOrWhiteSpace(text))
        return string.Empty;

    text = text.ToLower();

    var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
    var sb = new System.Text.StringBuilder();

    foreach (var c in normalized)
    {
        if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
            != System.Globalization.UnicodeCategory.NonSpacingMark)
        {
            sb.Append(c);
        }
    }

    text = sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
    text = System.Text.RegularExpressions.Regex.Replace(text, @"[^\w\s]", "");
    text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");

    return text.Trim();
}

    }
}
