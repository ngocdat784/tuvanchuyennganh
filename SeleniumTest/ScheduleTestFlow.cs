using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

class ScheduleTestFlow
{
    public static void Run()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            var js = (IJavaScriptExecutor)driver;

            // =========================
            // 1. LOGIN
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/pages/students/login.html");

            wait.Until(d => d.FindElement(By.Id("email"))).SendKeys("hala@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123");

            driver.FindElement(By.TagName("button")).Click();

            Console.WriteLine("Đang đăng nhập...");

            // đợi token
            wait.Until(d =>
            {
                var token = ((IJavaScriptExecutor)d)
                    .ExecuteScript("return localStorage.getItem('token');");
                return token != null;
            });

            Console.WriteLine("Login OK");

            // =========================
            // 2. VỀ TRANG CHỦ
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/index.html");

            // =========================
            // 3. SCROLL XUỐNG CHUYÊN VIÊN
            // =========================
            var counselorSection = wait.Until(d => d.FindElement(By.ClassName("counselor-section")));

            js.ExecuteScript("arguments[0].scrollIntoView({behavior:'smooth', block:'center'});", counselorSection);

            Console.WriteLine("Đã scroll xuống chuyên viên");

            // =========================
            // 4. CLICK HỒ SƠ CHUYÊN VIÊN
            // =========================
            wait.Until(d => d.FindElements(By.CssSelector(".btn-profile")).Count > 0);

            var profileBtn = driver.FindElements(By.CssSelector(".btn-profile"))[0];

            // scroll tới button
            js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", profileBtn);

            // click bằng JS tránh bị intercept
            js.ExecuteScript("arguments[0].click();", profileBtn);

            Console.WriteLine("Đã vào hồ sơ chuyên viên");

            // =========================
            // 5. CLICK ĐĂNG KÝ TƯ VẤN
            // =========================
            var bookBtn = wait.Until(d => d.FindElement(By.ClassName("btn-book")));

            js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", bookBtn);
            js.ExecuteScript("arguments[0].click();", bookBtn);

            Console.WriteLine("Đã chuyển sang trang đặt lịch");

            // =========================
            // 6. NHẬP FORM
            // =========================
            var dateInput = wait.Until(d => d.FindElement(By.Id("scheduleDate")));
            var timeInput = driver.FindElement(By.Id("scheduleTime"));
            var noteInput = driver.FindElement(By.Id("note"));

            // clear trước cho chắc
            dateInput.Clear();
            timeInput.Clear();
            noteInput.Clear();

            string date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            // dùng JS set value (ổn định hơn SendKeys)
            js.ExecuteScript($"arguments[0].value='{date}'", dateInput);
            js.ExecuteScript($"arguments[0].value='09:00'", timeInput);

            noteInput.SendKeys("Test Selenium booking");

            Console.WriteLine("Đã nhập form");

            // =========================
            // 7. SUBMIT
            // =========================
            var submitBtn = driver.FindElement(By.ClassName("btn-main"));

            js.ExecuteScript("arguments[0].scrollIntoView({block:'center'});", submitBtn);
            js.ExecuteScript("arguments[0].click();", submitBtn);

            Console.WriteLine("Đã gửi đặt lịch");

            // =========================
            // 8. HANDLE ALERT
            // =========================
            wait.Until(d =>
            {
                try
                {
                    var alert = d.SwitchTo().Alert();
                    Console.WriteLine("ALERT: " + alert.Text);
                    alert.Accept();
                    return true;
                }
                catch
                {
                    return false;
                }
            });

            Console.WriteLine("TEST PASS - Đặt lịch OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine("TEST FAIL: " + ex.Message);
        }
        finally
        {
            driver.Quit();
        }
    }
}