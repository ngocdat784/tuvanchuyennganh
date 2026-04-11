using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

class RoleTestFlow
{
    public static void Run()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        try
        {
            Console.WriteLine("===== TEST PHÂN QUYỀN =====");

            // =========================
            // 1. TEST LOGIN SAI
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/pages/students/login.html");

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email"))).SendKeys("sai@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123456");

            driver.FindElement(By.TagName("button")).Click();

            // đợi message lỗi
            wait.Until(d => d.FindElement(By.Id("msg")).Text.Length > 0);

            string errorMsg = driver.FindElement(By.Id("msg")).Text;
            Console.WriteLine("Login sai: " + errorMsg);

            // =========================
            // 2. TEST ADMIN
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/pages/students/login.html");

            driver.FindElement(By.Id("email")).Clear();
            driver.FindElement(By.Id("password")).Clear();

            driver.FindElement(By.Id("email")).SendKeys("admin@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123");

            driver.FindElement(By.TagName("button")).Click();

            // đợi login
            wait.Until(d =>
            {
                var token = ((IJavaScriptExecutor)d)
                    .ExecuteScript("return localStorage.getItem('token');");
                return token != null;
            });

            Console.WriteLine("Admin login thành công");

            // vào trang chủ
            driver.Navigate().GoToUrl("http://localhost:5086/index.html");

            // check admin dashboard
            wait.Until(ExpectedConditions.ElementIsVisible(
                By.CssSelector(".services.admin-dashboard")));

            Console.WriteLine("Admin thấy dashboard");

            // logout
            ((IJavaScriptExecutor)driver).ExecuteScript("localStorage.clear();");

            // =========================
            // 3. TEST STUDENT
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/pages/students/login.html");

            driver.FindElement(By.Id("email")).SendKeys("hala@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123");

            driver.FindElement(By.TagName("button")).Click();

            wait.Until(d =>
            {
                var token = ((IJavaScriptExecutor)d)
                    .ExecuteScript("return localStorage.getItem('token');");
                return token != null;
            });

            Console.WriteLine("Student login thành công");

            driver.Navigate().GoToUrl("http://localhost:5086/index.html");

            // check student section (dịch vụ)
            wait.Until(ExpectedConditions.ElementIsVisible(
                By.CssSelector(".student-only-section")));

            Console.WriteLine("Student thấy giao diện học sinh");

            Console.WriteLine("===== TEST PASS =====");
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