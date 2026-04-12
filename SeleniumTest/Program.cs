using System;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            ShowMenu();

            Console.Write("Chọn test: ");
            string choice = Console.ReadLine();

            if (choice == "0") return;

            RunTest(choice);

            Pause();
        }
    }

    // ================= MENU =================
    static void ShowMenu()
    {
        Console.Clear();

        Console.WriteLine("=================================");
        Console.WriteLine("       SELENIUM TEST MENU");
        Console.WriteLine("=================================");
        Console.WriteLine("1. Test Full Flow (Làm bài test)");
        Console.WriteLine("2. Test Phân quyền (Role)");
        Console.WriteLine("3. Test Đặt lịch tư vấn");
        Console.WriteLine("4. Test Admin CRUD ngành");
        Console.WriteLine("5. Test Admin CRUD nhóm câu hỏi");
        Console.WriteLine("0. Thoát");
        Console.WriteLine("=================================");
    }

    // ================= RUN TEST =================
    static void RunTest(string choice)
    {
        switch (choice)
        {
            case "1":
                RunWithTitle("Full Flow Test", FullTestFlow.Run);
                break;

            case "2":
                RunWithTitle("Role Test", RoleTestFlow.Run);
                break;

            case "3":
                RunWithTitle("Schedule Test", ScheduleTestFlow.Run);
                break;

            case "4":
                RunWithTitle("Admin CRUD Major Test", ManagerMajorsTestFlow.Run);
                break;

            case "5":
                RunWithTitle("Admin CRUD Question Group Test", QuestionGroupTestFlow.Run);
                break;

            default:
                Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                break;
        }
    }

    // ================= WRAPPER =================
    static void RunWithTitle(string title, Action testMethod)
    {
        Console.WriteLine($"\n===== {title.ToUpper()} =====");

        try
        {
            testMethod();
            Console.WriteLine("✅ TEST PASS");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ TEST FAIL: " + ex.Message);
        }
    }

    // ================= PAUSE =================
    static void Pause()
    {
        Console.WriteLine("\nNhấn phím bất kỳ để quay lại menu...");
        Console.ReadKey();
    }
}