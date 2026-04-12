using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;

class ManagerMajorsTestFlow
{
    public static void Run()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        var js = (IJavaScriptExecutor)driver;

        try
        {
            Login(driver, wait);
            GoToMajorPage(driver, wait, js);

            string testName = CreateMajor(driver, wait, js);
            string updatedName = UpdateMajor(driver, wait, js, testName);

            DeleteMajor(driver, wait, js, updatedName);

            Console.WriteLine("TEST PASS - CRUD MAJOR FULL FLOW OK");
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

    // ================= LOGIN =================
    static void Login(IWebDriver driver, WebDriverWait wait)
    {
        driver.Navigate().GoToUrl("http://localhost:5086/pages/students/login.html");

        wait.Until(d => d.FindElement(By.Id("email"))).SendKeys("admin@gmail.com");
        driver.FindElement(By.Id("password")).SendKeys("123");
        driver.FindElement(By.TagName("button")).Click();

        wait.Until(d =>
        {
            var token = ((IJavaScriptExecutor)d)
                .ExecuteScript("return localStorage.getItem('token');");
            return token != null;
        });

        Console.WriteLine("Login admin OK");
    }

    // ================= NAVIGATE =================
    static void GoToMajorPage(IWebDriver driver, WebDriverWait wait, IJavaScriptExecutor js)
    {
        driver.Navigate().GoToUrl("http://localhost:5086/index.html");

        var card = wait.Until(d =>
            d.FindElements(By.XPath("//h3[contains(text(),'Quản lý ngành')]")).FirstOrDefault()
        );

        js.ExecuteScript("arguments[0].scrollIntoView(true);", card);
        js.ExecuteScript("arguments[0].click();", card);

        Console.WriteLine("Đã vào trang quản lý ngành");
    }

    // ================= CREATE =================
    static string CreateMajor(IWebDriver driver, WebDriverWait wait, IJavaScriptExecutor js)
    {
        var addBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//button[contains(text(),'Thêm ngành')]")
        ));
        js.ExecuteScript("arguments[0].click();", addBtn);

        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("addName")));

        string testName = "Ngành Test " + DateTime.Now.Ticks;

        driver.FindElement(By.Id("addName")).SendKeys(testName);
        driver.FindElement(By.Id("addDescription")).SendKeys("Mô tả test");
        driver.FindElement(By.Id("addDetails")).SendKeys("Chi tiết test");

        ClickByJS(driver, "//button[contains(text(),'Lưu')]");

        WaitForText(driver, wait, testName);

        Console.WriteLine("Create OK");
        return testName;
    }

    // ================= UPDATE =================
    static string UpdateMajor(IWebDriver driver, WebDriverWait wait, IJavaScriptExecutor js, string testName)
    {
        ScrollToBottom(js);
        var row = FindRow(driver, testName);

        ClickByJS(driver, row.FindElement(By.ClassName("btn-edit")));

        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("editName")));

        var input = driver.FindElement(By.Id("editName"));
        input.Clear();

        string updatedName = testName + " Updated";
        input.SendKeys(updatedName);

        ClickByJS(driver, "//button[contains(text(),'Cập nhật')]");

        WaitForText(driver, wait, updatedName);

        Console.WriteLine("Update OK");
        return updatedName;
    }

    // ================= DELETE =================
    static void DeleteMajor(IWebDriver driver, WebDriverWait wait, IJavaScriptExecutor js, string updatedName)
    {
        ScrollToBottom(js);
        var row = FindRow(driver, updatedName);

        ClickByJS(driver, row.FindElement(By.ClassName("btn-delete")));

        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("deleteModal")));

        ClickByJS(driver, "//button[contains(text(),'Xóa')]");

        wait.Until(d =>
            d.FindElements(By.XPath($"//td[contains(text(),'{updatedName}')]")).Count == 0
        );

        Console.WriteLine("Delete OK");
    }

    // ================= HELPER =================

    static void ClickByJS(IWebDriver driver, string xpath)
    {
        var element = driver.FindElement(By.XPath(xpath));
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
    }

    static void ClickByJS(IWebDriver driver, IWebElement element)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
    }

    static void ScrollToBottom(IJavaScriptExecutor js)
    {
        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        System.Threading.Thread.Sleep(500);
    }

    static IWebElement FindRow(IWebDriver driver, string text)
    {
        return driver.FindElement(By.XPath($"//tr[td[contains(text(),'{text}')]]"));
    }

    static void WaitForText(IWebDriver driver, WebDriverWait wait, string text)
    {
        wait.Until(d =>
            d.FindElements(By.XPath($"//td[contains(text(),'{text}')]")).Count > 0
        );
    }
}