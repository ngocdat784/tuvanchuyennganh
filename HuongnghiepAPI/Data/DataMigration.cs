using CareerOrientationAPI.Models;

namespace CareerOrientationAPI.Data
{
    public static class DataMigration
    {
        public static void MigrateAllData(OldDbContext oldDb, AppDbContext newDb)
        {
            // -----------------------
            // SIMPLE TABLES
            // -----------------------

            newDb.Admins.AddRange(oldDb.Admins.ToList());
            newDb.Students.AddRange(oldDb.Students.ToList());
            newDb.Counselors.AddRange(oldDb.Counselors.ToList());
            newDb.Notifications.AddRange(oldDb.Notifications.ToList());

            // -----------------------
            // MAJOR + RELATED
            // -----------------------

            newDb.Majors.AddRange(oldDb.Majors.ToList());
            newDb.MajorTraits.AddRange(oldDb.MajorTraits.ToList());
            newDb.MajorSubFields.AddRange(oldDb.MajorSubFields.ToList());

            // -----------------------
            // UNIVERSITY
            // -----------------------

            newDb.Universities.AddRange(oldDb.Universities.ToList());
            newDb.UniversityMajors.AddRange(oldDb.UniversityMajors.ToList());

            // -----------------------
            // QUESTION SYSTEM
            // -----------------------

            newDb.QuestionGroups.AddRange(oldDb.QuestionGroups.ToList());
            newDb.Questions.AddRange(oldDb.Questions.ToList());
            newDb.Answers.AddRange(oldDb.Answers.ToList());
            newDb.AnswerImpacts.AddRange(oldDb.AnswerImpacts.ToList());

            // -----------------------
            // TEST SYSTEM
            // -----------------------

            newDb.TestResults.AddRange(oldDb.TestResults.ToList());
            newDb.TestAnswers.AddRange(oldDb.TestAnswers.ToList());
            newDb.TestAnswerImpacts.AddRange(oldDb.TestAnswerImpacts.ToList());
            newDb.TestResultMajorScores.AddRange(oldDb.TestResultMajorScores.ToList());

            // -----------------------
            // ROADMAP
            // -----------------------

            newDb.Roadmaps.AddRange(oldDb.Roadmaps.ToList());
            newDb.RoadmapStages.AddRange(oldDb.RoadmapStages.ToList());
            newDb.RoadmapItems.AddRange(oldDb.RoadmapItems.ToList());
            newDb.RoadmapSkills.AddRange(oldDb.RoadmapSkills.ToList());

            // -----------------------
            // SCHEDULE
            // -----------------------

            newDb.CounselingSchedules.AddRange(oldDb.CounselingSchedules.ToList());

            // -----------------------
            // SAVE
            // -----------------------

            newDb.SaveChanges();
        }
    }
}