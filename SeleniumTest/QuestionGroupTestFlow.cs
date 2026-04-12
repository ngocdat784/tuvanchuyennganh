using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;

class QuestionGroupTestFlow
{
    public static void Run()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        var js = (IJavaScriptExecutor)driver;

        try
        {
            // =========================
            // 1. LOGIN ADMIN
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/pages/students/login.html");

            wait.Until(d => d.FindElement(By.Id("email"))).SendKeys("admin@gmail.com");
            driver.FindElement(By.Id("password")).SendKeys("123");
            driver.FindElement(By.TagName("button")).Click();

            wait.Until(d =>
            {
                var token = js.ExecuteScript("return localStorage.getItem('token');");
                return token != null;
            });

            Console.WriteLine("Login OK");

            // =========================
            // 2. VÀO TRANG CHÍNH
            // =========================
            driver.Navigate().GoToUrl("http://localhost:5086/index.html");

            // =========================
            // 3. CLICK QUẢN LÝ NHÓM
            // =========================
            var groupCard = wait.Until(d =>
                d.FindElements(By.XPath("//h3[contains(text(),'Quản lý nhóm')]")).FirstOrDefault()
            );

            js.ExecuteScript("arguments[0].click();", groupCard);

            Console.WriteLine("Vào trang group");

            // =========================
            // 4. CLICK THÊM (MODAL)
            // =========================
            var addBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(text(),'Thêm nhóm')]")
            ));

            js.ExecuteScript("arguments[0].click();", addBtn);

            // =========================
            // 5. NHẬP DATA CREATE
            // =========================
            string testName = "Group Test " + DateTime.Now.Ticks;

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("addTitle")));

            driver.FindElement(By.Id("addTitle")).SendKeys(testName);
            driver.FindElement(By.Id("addDescription")).SendKeys("Mô tả test");
            driver.FindElement(By.Id("addOrder")).SendKeys("1");

            var saveBtn = driver.FindElement(By.XPath("//button[contains(text(),'Lưu')]"));
            js.ExecuteScript("arguments[0].click();", saveBtn);

            Console.WriteLine("Đã tạo group");

            // =========================
            // 6. CHỜ TABLE UPDATE
            // =========================
            wait.Until(d =>
                d.FindElements(By.XPath($"//td[contains(text(),'{testName}')]")).Count > 0
            );

            // scroll xuống cuối
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            System.Threading.Thread.Sleep(500);

            // =========================
            // 7. TÌM ROW
            // =========================
            var row = driver.FindElement(By.XPath($"//tr[td[contains(text(),'{testName}')]]"));

            // =========================
            // 8. CLICK EDIT
            // =========================
            var editBtn = row.FindElement(By.ClassName("btn-edit"));
            js.ExecuteScript("arguments[0].click();", editBtn);

            Console.WriteLine("Mở edit");

            // =========================
            // 9. UPDATE
            // =========================
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("editTitle")));

            var titleInput = driver.FindElement(By.Id("editTitle"));
            titleInput.Clear();

            string updatedName = testName + " Updated";
            titleInput.SendKeys(updatedName);

            var updateBtn = driver.FindElement(By.XPath("//button[contains(text(),'Cập nhật')]"));
            js.ExecuteScript("arguments[0].click();", updateBtn);

            Console.WriteLine("Đã update");

            // =========================
            // 10. VERIFY UPDATE
            // =========================
            wait.Until(d =>
                d.FindElements(By.XPath($"//td[contains(text(),'{updatedName}')]")).Count > 0
            );

            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            System.Threading.Thread.Sleep(500);

            var updatedRow = driver.FindElement(By.XPath($"//tr[td[contains(text(),'{updatedName}')]]"));

            // =========================
            // 11. CLICK DELETE
            // =========================
            var deleteBtn = updatedRow.FindElement(By.ClassName("btn-delete"));
            js.ExecuteScript("arguments[0].click();", deleteBtn);

            Console.WriteLine("Mở modal delete");

            // =========================
            // 12. CONFIRM DELETE
            // =========================
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("deleteModal")));

            var confirmDelete = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(text(),'Xóa')]")
            ));

            js.ExecuteScript("arguments[0].click();", confirmDelete);

            // =========================
            // 13. VERIFY DELETE
            // =========================
            wait.Until(d =>
                d.FindElements(By.XPath($"//td[contains(text(),'{updatedName}')]")).Count == 0
            );

            Console.WriteLine("TEST PASS - CRUD QUESTION GROUP OK");
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