using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using CareerOrientationAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db")
);
builder.Services.AddDbContext<OldDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
#pragma warning disable CS8604 // Possible null reference argument.
options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
#pragma warning restore CS8604 // Possible null reference argument.
});

// Controllers + JSON Fix
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var newDb = services.GetRequiredService<AppDbContext>();
    newDb.Database.Migrate();

    // ❗ CHỈ chạy migrate data ở LOCAL (Development)
    if (app.Environment.IsDevelopment())
    {
        var oldDb = services.GetRequiredService<OldDbContext>();

        if (!newDb.Admins.Any())
        {
            Console.WriteLine(">>> MIGRATING DATA FROM SQL SERVER...");
            DataMigration.MigrateAllData(oldDb, newDb);
        }
    }

    DbInitializer.SeedAdmin(newDb);
}

app.UseCors("AllowFrontend");
app.UseAuthentication(); // ⛔ PHẢI TRƯỚC
app.UseAuthorization();  // ⛔ PHẢI SAU
app.UseDefaultFiles();   // 👈 Tự động tìm index.html
app.UseStaticFiles();

app.MapControllers();

app.Run();
