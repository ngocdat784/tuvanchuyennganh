using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

class FullTestFlow
{
    static void Main()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

        try
        {
            // =========================
            // 1. MỞ TRANG LOGIN
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/pages/students/login.html");

            // =========================
            // 2. NHẬP LOGIN
            // =========================
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email"))).SendKeys("hala@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123");

            driver.FindElement(By.TagName("button")).Click();

            Console.WriteLine("Đang đăng nhập...");

            // =========================
            // 3. ĐỢI LOGIN XONG (token)
            // =========================
            wait.Until(d =>
            {
                var token = ((IJavaScriptExecutor)d)
                    .ExecuteScript("return localStorage.getItem('token');");
                return token != null;
            });

            Console.WriteLine("Login thành công");

            // =========================
            // 4. VÀO TRANG TEST
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/pages/students/start-test.html");

            // =========================
            // 5. CLICK BẮT ĐẦU (FIX)
            // =========================
            var startBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("startTestBtn")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", startBtn);

            Console.WriteLine("Đã bắt đầu test");

            // =========================
            // 6. ĐỢI LOAD CÂU HỎI
            // =========================
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("questionTitle")));

            // =========================
            // 7. LOOP TRẢ LỜI (FIX QUAN TRỌNG)
            // =========================
          while (true)
{
    // lấy câu hiện tại
    var question = driver.FindElement(By.Id("questionTitle"));
    string oldQuestion = question.Text;

    // đợi đáp án mới load
    wait.Until(d => d.FindElements(By.CssSelector("#answerList label")).Count > 0);

    var labels = driver.FindElements(By.CssSelector("#answerList label"));

    // click đáp án
    ((IJavaScriptExecutor)driver)
        .ExecuteScript("arguments[0].click();", labels[0]);

    Console.WriteLine("Đã chọn đáp án");

    // NEXT
    var next = driver.FindElements(By.Id("nextBtn"));
    if (next.Count > 0 && next[0].Displayed)
    {
        ((IJavaScriptExecutor)driver)
            .ExecuteScript("arguments[0].click();", next[0]);

        // đợi câu mới (FIX QUAN TRỌNG NHẤT)
        wait.Until(d =>
        {
            var newText = d.FindElement(By.Id("questionTitle")).Text;
            return newText != oldQuestion;
        });

        continue;
    }

    // FINISH
    var finish = driver.FindElements(By.Id("finishBtn"));
    if (finish.Count > 0 && finish[0].Displayed)
    {
        ((IJavaScriptExecutor)driver)
            .ExecuteScript("arguments[0].click();", finish[0]);
        break;
    }
}
            Console.WriteLine("Đã hoàn thành bài test");

            // =========================
            // 8. ĐỢI KẾT QUẢ
            // =========================
            wait.Until(d => d.PageSource.Contains("Kết quả bài test"));

            Console.WriteLine("TEST PASS - Hệ thống hoạt động OK");
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