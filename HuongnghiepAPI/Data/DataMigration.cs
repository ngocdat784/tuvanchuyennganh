using CareerOrientationAPI.Data;
using Microsoft.EntityFrameworkCore;

public class DataMigration
{
    public static void Migrate(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var oldDb = scope.ServiceProvider.GetRequiredService<OldDbContext>();
        var newDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Console.WriteLine("=== START MIGRATION ===");

        // ❗ XÓA DATA CŨ (tránh duplicate)
        newDb.Database.ExecuteSqlRaw("DELETE FROM AnswerImpacts");
        newDb.Database.ExecuteSqlRaw("DELETE FROM Answers");
        newDb.Database.ExecuteSqlRaw("DELETE FROM Questions");
        newDb.Database.ExecuteSqlRaw("DELETE FROM QuestionGroups");
        newDb.Database.ExecuteSqlRaw("DELETE FROM MajorTraits");
        newDb.Database.ExecuteSqlRaw("DELETE FROM MajorSubFields");
        newDb.Database.ExecuteSqlRaw("DELETE FROM UniversityMajors");
        newDb.Database.ExecuteSqlRaw("DELETE FROM Universities");
        newDb.Database.ExecuteSqlRaw("DELETE FROM Majors");

        newDb.SaveChanges();

        // =========================
        // COPY THEO THỨ TỰ CHUẨN
        // =========================

        // 1. Majors
        var majors = oldDb.Majors.AsNoTracking().ToList();
        newDb.Majors.AddRange(majors);
        newDb.SaveChanges();
        Console.WriteLine("Majors done");

        // 2. Universities
        var universities = oldDb.Universities.AsNoTracking().ToList();
        newDb.Universities.AddRange(universities);
        newDb.SaveChanges();
        Console.WriteLine("Universities done");

        // 3. UniversityMajors
        var uniMajors = oldDb.UniversityMajors.AsNoTracking().ToList();
        newDb.UniversityMajors.AddRange(uniMajors);
        newDb.SaveChanges();

        // 4. MajorTraits
        var traits = oldDb.MajorTraits.AsNoTracking().ToList();
        newDb.MajorTraits.AddRange(traits);
        newDb.SaveChanges();

        // 5. MajorSubFields
        var subFields = oldDb.MajorSubFields.AsNoTracking().ToList();
        newDb.MajorSubFields.AddRange(subFields);
        newDb.SaveChanges();

        // 6. QuestionGroups
        var groups = oldDb.QuestionGroups.AsNoTracking().ToList();
        newDb.QuestionGroups.AddRange(groups);
        newDb.SaveChanges();

        // 7. Questions
        var questions = oldDb.Questions.AsNoTracking().ToList();
        newDb.Questions.AddRange(questions);
        newDb.SaveChanges();

        // 8. Answers
        var answers = oldDb.Answers.AsNoTracking().ToList();
        newDb.Answers.AddRange(answers);
        newDb.SaveChanges();

        // 9. AnswerImpacts
        var impacts = oldDb.AnswerImpacts.AsNoTracking().ToList();
        newDb.AnswerImpacts.AddRange(impacts);
        newDb.SaveChanges();

        // =========================
        // TEST DATA (OPTIONAL)
        // =========================

        var students = oldDb.Students.AsNoTracking().ToList();
        newDb.Students.AddRange(students);

        var counselors = oldDb.Counselors.AsNoTracking().ToList();
        newDb.Counselors.AddRange(counselors);

        newDb.SaveChanges();

        Console.WriteLine("=== MIGRATION DONE ===");
    }
}