using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SpurFlow.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using TechTalk.SpecFlow;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace SpurFlow.Utils
{
    public static class WebDriverUtil
    {
        private static IWebDriver _driver;
        private static int timeoutInSeconds = Int32.Parse(CommonUtil.getFrameworkConfig("webdriver_explicit_wait"));

        public static IWebElement FindWebElement(this IWebDriver driver, By by)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        public static ReadOnlyCollection<IWebElement> FindWebElements(this IWebDriver driver, By by)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => (drv.FindElements(by).Count > 0) ? drv.FindElements(by) : null);
            }
            return driver.FindElements(by);
        }

        public static IWebDriver WebInit()
        {
            string executionMode = CommonUtil.getFrameworkConfig("execution_mode");
            string driverExecutablePath = CommonUtil.getFrameworkConfig("driver_exe_path");
            string browser = CommonUtil.getFrameworkConfig("browser");

            if (executionMode == "Local")
            {
                switch (browser)
                {
                    case "Chrome":
                        
                        _driver = new ChromeDriver("../Resources/DriverExe");
                        break;
                    case "Headless":
                        Console.WriteLine("to be implemented");
                        break;
                    case "IE":
                        _driver = new InternetExplorerDriver(@driverExecutablePath);
                        break;
                    case "Firefox":
                        _driver = new FirefoxDriver(@driverExecutablePath);
                        break;
                    default:
                        Console.WriteLine("Please update config.json with correct browsername");
                        break;
                }

            }
            else if (executionMode == "Cloud_Web")
            {
          /*      DesiredCapabilities capability = new DesiredCapabilities();
                capability.SetCapability("os", CommonUtil.getFrameworkConfig("os"));
                capability.SetCapability("os_version", CommonUtil.getFrameworkConfig("os_version"));
                capability.SetCapability("browser", CommonUtil.getFrameworkConfig("browser"));
                capability.SetCapability("resolution", CommonUtil.getFrameworkConfig("resolution"));
                capability.SetCapability("browser_version", CommonUtil.getFrameworkConfig("browser_version"));
                capability.SetCapability("browserstack.local", CommonUtil.getFrameworkConfig("browserstack.local"));
                capability.SetCapability("browserstack.selenium_version", CommonUtil.getFrameworkConfig("browserstack.selenium_version"));
                capability.SetCapability("browserstack.user", CommonUtil.getFrameworkConfig("browserstack.user"));
                capability.SetCapability("browserstack.key", CommonUtil.getFrameworkConfig("browserstack.key"));
                capability.SetCapability("browserstack.geoLocation", CommonUtil.getFrameworkConfig("browserstack.geoLocation"));
                _driver = new RemoteWebDriver(
                  new Uri(CommonUtil.getFrameworkConfig("Account_hub_URL")), capability);
*/
            }
            else if (executionMode == "Cloud_Mobile")
            {
            /*    DesiredCapabilities capability = new DesiredCapabilities();
                capability.SetCapability("os_version", CommonUtil.getFrameworkConfig("device_os_version"));
                capability.SetCapability("device", CommonUtil.getFrameworkConfig("device"));
                capability.SetCapability("real_mobile", CommonUtil.getFrameworkConfig("real_mobile"));
                capability.SetCapability("browserstack.local", CommonUtil.getFrameworkConfig("browserstack.local"));
                capability.SetCapability("browserstack.user", CommonUtil.getFrameworkConfig("browserstack.user"));
                capability.SetCapability("browserstack.key", CommonUtil.getFrameworkConfig("browserstack.key"));
                capability.SetCapability("browserstack.selenium_version", CommonUtil.getFrameworkConfig("browserstack.selenium_version"));
                capability.SetCapability("browserstack.geoLocation", CommonUtil.getFrameworkConfig("browserstack.geoLocation"));
                _driver = new RemoteWebDriver(
                  new Uri(CommonUtil.getFrameworkConfig("Account_hub_URL")), capability);*/
            }
            _driver.Navigate().GoToUrl(CommonUtil.getAppConfig("app_URL"));
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeoutInSeconds);
            return _driver;

        }

        public static void WebTearDown()
        {
            if (!_driver.ToString().Contains("null"))
            {
                _driver.Close();
                _driver.Quit();

            }
        }

        public static string SaveScreenShot(string fileName)
        {
            var takesScreenshot = _driver as ITakesScreenshot;
            if (takesScreenshot != null)
            {
                var screenshot = takesScreenshot.GetScreenshot();
                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
                return fileName;
            }
            return null;

        }

        public static string TakeScreenShot()
        {
            string filename = CommonUtil.getFrameworkConfig("screenshot_path") + ScenarioStepContext.Current.StepInfo.StepDefinitionType + ".png";

            var takesScreenshot = _driver as ITakesScreenshot;
            if (takesScreenshot != null)
            {
                var screenshot = takesScreenshot.GetScreenshot();
                screenshot.SaveAsFile(filename, ScreenshotImageFormat.Png);
                return filename;
            }
            return null;

        }
        public static string DesktopScreenShot(string fileName)
        {
            Bitmap memoryImage;
            memoryImage = new Bitmap(1600, 900);
            Size s = new Size(memoryImage.Width, memoryImage.Height);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
            try
            {
                memoryImage.Save(fileName);
                return fileName;

            }
            catch (Exception er)
            {
                Console.WriteLine("Sorry, there was an error: " + er.Message);
                return null;

            }
        }
        public static bool IsElementPresent(this IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static IList<string> GetList(IList<IWebElement> elements)
        {

            IList<IWebElement> element = (IList<IWebElement>)elements;

            IList<string> alltext = new List<string>();
            foreach (IWebElement allelement in element)
            {
                string s = allelement.Text;
                alltext.Add(s);
            }
            return alltext;
        }

        public static PageInfo getPageInfo(IWebDriver driver)
        {
            /*
            PageInfo pageInfo = new PageInfo();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            pageInfo.PageURL = driver.Url;
            pageInfo.domContentLoadedEventEnd = Convert.ToInt64(js.ExecuteScript("return window.performance.timing.domContentLoadedEventEnd"));
            pageInfo.domComplete = Convert.ToInt64(js.ExecuteScript("return window.performance.timing.domComplete"));
            pageInfo.responseStart = Convert.ToInt64(js.ExecuteScript("return window.performance.timing.responseStart"));
            pageInfo.navigationStart = Convert.ToInt64(js.ExecuteScript("return window.performance.timing.navigationStart"));
            ScenarioContext.Current.StepContext.Set(pageInfo, "PageInfo");
            return pageInfo;
            */
            PageInfo pageInfo = new PageInfo();

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            //pageInfo = JsonConvert.DeserializeObject<PageInfo>

            Object response = js.ExecuteScript(jsGetTiming());

            pageInfo = JsonConvert.DeserializeObject<PageInfo>(response.ToString());
            return pageInfo;
        }


        public static string jsGetTiming()
        {

            // func is the javascript function that will get executed
            string func = @"var perfEntries = performance.getEntriesByType('navigation');
            var entry = perfEntries[0];

            var json = entry.toJSON(); 
            var s = JSON.stringify(json);
            
            return s;";

            return func;
        }


    }
 

    public class PageInfo
    {
        public string name { get; set; }
        public string entryType { get; set; }
        public float startTime { get; set; }
        public float duration { get; set; }
        public string initiatorType { get; set; }
        public string nextHopProtocol { get; set; }
        public float workerStart { get; set; }
        public float redirectStart { get; set; }
        public float redirectEnd { get; set; }
        public float fetchStart { get; set; }
        public float domainLookupStart { get; set; }
        public float domainLookupEnd { get; set; }
        public float connectStart { get; set; }
        public float connectEnd { get; set; }
        public float secureConnectionStart { get; set; }
        public float requestStart { get; set; }
        public float responseStart { get; set; }
        public float responseEnd { get; set; }
        public int transferSize { get; set; }
        public int encodedBodySize { get; set; }
        public int decodedBodySize { get; set; }
        public object[] serverTiming { get; set; }
        public object[] workerTiming { get; set; }
        public float unloadEventStart { get; set; }
        public float unloadEventEnd { get; set; }
        public float domInteractive { get; set; }
        public float domContentLoadedEventStart { get; set; }
        public float domContentLoadedEventEnd { get; set; }
        public float domComplete { get; set; }
        public float loadEventStart { get; set; }
        public float loadEventEnd { get; set; }
        public string type { get; set; }
        public int redirectCount { get; set; }
    }

}
