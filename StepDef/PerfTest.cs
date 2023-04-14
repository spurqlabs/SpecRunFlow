using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SpurFlow.Pages;
using SpurFlow.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace SpurFlow.StepDef
{
    public class PerfTest
    {
        [Fact]
        public void testpageloadtime()
        {

            ChromeOptions option = new ChromeOptions();
            option.AddArgument("no-sandbox");
            IWebDriver driver = new ChromeDriver(@"../../../Resources/DriverExe/", option, TimeSpan.FromSeconds(130));
            //IWebDriver driver = new ChromeDriver(@"../../../Resources/DriverExe/");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            //SignInPage signInPage = new SignInPage(driver, wait);
            driver.Navigate().GoToUrl("https://www.google.com");
          //  signInPage.VerifyHomePage();
            PageInfo pageInfo = WebDriverUtil.getPageInfo(driver);
            var pages = new List<PageInfo>();

            pages.Add(pageInfo);
            var output = GenerateReport(pages);
            File.WriteAllText(@"../../../TestResults/PageLoadTime.csv", output);
            Assert.Equal("https://www.google.com/", pageInfo.name);
            Assert.Equal(234729834728, pageInfo.domComplete);

            //long backendPerformance_calc = WebDriverUtil.getresponseStart(driver) - WebDriverUtil.getnavigationStart(driver);
            // long frontendPerformance_calc = WebDriverUtil.getdomComplete(driver) - WebDriverUtil.getresponseStart(driver);
            // Assert.Equal(23423423, backendPerformance_calc);
            // Assert.Equal(2342342, frontendPerformance_calc);

            // long pageloadtime = WebDriverUtil.getdomContentLoadedEventEnd(driver) - WebDriverUtil.getnavigationStart(driver); 3709
            //Assert.Equal(2238092834929384, pageloadtime);


           // signInPage.ClickLoginButton();
            string user = CommonUtil.getAppConfig("user");
            string password = CommonUtil.getAppConfig("Password");
          //  signInPage.LoginWithUserPassword(user, password);
            driver.Quit();
        


        }
      
        public string GenerateReport<T>(List<T> items) where T : class
        {
            var output = "";
            var delimiter = ',';
            var properties = typeof(T).GetProperties().Where(n =>
             n.PropertyType == typeof(string)
             || n.PropertyType == typeof(bool)
             || n.PropertyType == typeof(char)
             || n.PropertyType == typeof(byte)
             || n.PropertyType == typeof(decimal)
             || n.PropertyType == typeof(int)
             || n.PropertyType == typeof(float)
             || n.PropertyType == typeof(object[])
             || n.PropertyType == typeof(DateTime)
             || n.PropertyType == typeof(DateTime?));
            using (var sw = new StringWriter())
            {
                var header = properties
                .Select(n => n.Name)
                .Aggregate((a, b) => a + delimiter + b);
                sw.WriteLine(header);
                foreach (var item in items)
                {
                    var row = properties
                    .Select(n => n.GetValue(item, null))
                    .Select(n => n == null ? "null" : n.ToString())
 .Aggregate((a, b) => a + delimiter + b);
                    sw.WriteLine(row);
                }
                output = sw.ToString();
            }
            return output;
        }
    }
}
