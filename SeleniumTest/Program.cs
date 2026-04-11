using System;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=================================");
            Console.WriteLine("   SELENIUM TEST MENU");
            Console.WriteLine("=================================");
            Console.WriteLine("1. Test Full Flow (Làm bài test)");
            Console.WriteLine("2. Test Phân quyền (Role)");
            Console.WriteLine("0. Thoát");
            Console.WriteLine("=================================");
            Console.Write("Chọn test: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Đang chạy Full Flow...");
                    FullTestFlow.Run();
                    Pause();
                    break;

                case "2":
                    Console.WriteLine("Đang chạy Role Test...");
                    RoleTestFlow.Run();
                    Pause();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    Pause();
                    break;
            }
        }
    }

    static void Pause()
    {
        Console.WriteLine("\nNhấn phím bất kỳ để quay lại menu...");
        Console.ReadKey();
    }
}