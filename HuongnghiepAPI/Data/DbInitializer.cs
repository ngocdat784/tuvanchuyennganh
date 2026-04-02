using CareerOrientationAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace CareerOrientationAPI.Data
{
    public static class DbInitializer
    {
        public static void SeedAdmin(AppDbContext context)
        {
            // ✅ Nếu đã có admin thì không tạo nữa
            if (context.Admins.Any())
                return;

            // Hash password
            string password = "admin123"; // đổi khi deploy
            string hashed = HashPassword(password);

            var admin = new Admin
            {
                FullName = "System Admin",
                Email = "admin@gmail.com",
                PasswordHash = hashed,
                Role = "Admin"
            };

            context.Admins.Add(admin);
            context.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }
}
