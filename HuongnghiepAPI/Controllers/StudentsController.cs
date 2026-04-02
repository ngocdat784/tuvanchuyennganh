using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CareerOrientationAPI.Data;
using CareerOrientationAPI.Models;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace CareerOrientationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public StudentController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ===============================
        // 🔐 Hash mật khẩu SHA256
        // ===============================
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        // ===============================
// 🔥 Tạo JWT Token (Chuẩn phân quyền)
// ===============================
private string GenerateJwtToken(
    int userId,
    string fullName,
    string email,
    string role
)
{
#pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"])
    );
#pragma warning restore CS8604 // Possible null reference argument.

            var creds = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256
    );

    var claims = new List<Claim>
    {
        new Claim("userId", userId.ToString()),
        new Claim(ClaimTypes.Name, fullName),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role)
    };

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(6),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] Student input)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    bool exists = await _db.Students.AnyAsync(s => s.Email == input.Email);
    if (exists)
        return Conflict(new { message = "Email này đã được sử dụng." });

    var student = new Student
    {
        FullName = input.FullName,
        Email = input.Email,
        PasswordHash = HashPassword(input.PasswordHash),
        School = input.School,
        Grade = input.Grade,
        Gender = input.Gender,
        BirthDate = input.BirthDate,
        Role = "student"
    };

    _db.Students.Add(student);
    await _db.SaveChangesAsync();

    return Ok(new
    {
        message = "Đăng ký thành công!",
        studentId = student.StudentId
    });
}
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto input)
{
    string hashed = HashPassword(input.Password);

    // ===============================
    // 1️⃣ ADMIN
    // ===============================
    var admin = await _db.Admins.FirstOrDefaultAsync(x =>
        x.Email == input.Email &&
        x.PasswordHash == hashed
    );

    if (admin != null)
    {
        var token = GenerateJwtToken(
            admin.AdminId,
            admin.FullName,
            admin.Email,
            admin.Role // "Admin"
        );

        return Ok(new
        {
            message = "Đăng nhập Admin thành công",
            token,
            role = admin.Role,
            userId = admin.AdminId,
            fullName = admin.FullName
        });
    }

    // ===============================
    // 2️⃣ COUNSELOR
    // ===============================
    var counselor = await _db.Counselors.FirstOrDefaultAsync(x =>
        x.Email == input.Email &&
        x.PasswordHash == hashed
    );

    if (counselor != null)
    {
        var token = GenerateJwtToken(
            counselor.CounselorId,
            counselor.FullName,
            counselor.Email,
            counselor.Role // "counselor"
        );

        return Ok(new
        {
            message = "Đăng nhập Counselor thành công",
            token,
            role = counselor.Role,
            userId = counselor.CounselorId,
            fullName = counselor.FullName
        });
    }

    // ===============================
    // 3️⃣ STUDENT
    // ===============================
    var student = await _db.Students.FirstOrDefaultAsync(x =>
        x.Email == input.Email &&
        x.PasswordHash == hashed
    );

    if (student != null)
    {
        var token = GenerateJwtToken(
            student.StudentId,
            student.FullName,
            student.Email,
            student.Role // "student"
        );

        return Ok(new
        {
            message = "Đăng nhập Student thành công",
            token,
            role = student.Role,
            userId = student.StudentId,
            fullName = student.FullName
        });
    }

    return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
}

        // ===============================
        // 🔵 Đăng xuất
        // ===============================
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Đăng xuất thành công (xoá token phía client)!" });
        }

        // ===============================
        // GET: api/student
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _db.Students
                .Select(s => new
                {
                    s.StudentId,
                    s.FullName,
                    s.Email,
                    s.School,
                    s.Grade,
                    s.Gender,
                    s.BirthDate
                })
                .ToListAsync();

            return Ok(students);
        }

        // ===============================
        // GET: api/student/{id}
        // ===============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _db.Students
                .Where(s => s.StudentId == id)
                .Select(s => new
                {
                    s.StudentId,
                    s.FullName,
                    s.Email,
                    s.School,
                    s.Grade,
                    s.Gender,
                    s.BirthDate
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound(new { message = "Không tìm thấy học sinh." });

            return Ok(student);
        }

        // ===============================
        // PUT: api/student/{id}
        // ===============================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student update)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Không tìm thấy học sinh." });

            student.FullName = update.FullName;
            student.School = update.School;
            student.Grade = update.Grade;
            student.Gender = update.Gender;
            student.BirthDate = update.BirthDate;

            if (!string.IsNullOrEmpty(update.PasswordHash))
            {
                student.PasswordHash = HashPassword(update.PasswordHash);
            }

            _db.Students.Update(student);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công." });
        }

        // ===============================
        // DELETE: api/student/{id}
        // ===============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Không tìm thấy học sinh." });

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Xoá học sinh thành công." });
        }
    }
    
}
public class LoginDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
