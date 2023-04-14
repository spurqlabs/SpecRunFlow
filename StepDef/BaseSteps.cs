using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using BoDi;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using Selenium.Axe;
using SpurFlow.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;
using static SpurFlow.Utils.APIUtil;

namespace SpurFlow.StepDef
{
    [Binding]
    public class BaseSteps : Steps
    {
        private static readonly AventStack.ExtentReports.ExtentReports Reporter = new AventStack.ExtentReports.ExtentReports();
        private static ExtentTest FeatureName;
        private static ExtentTest ScenarioName;
        private IObjectContainer _objectContainer;
        private IWebDriver _driver;
        private APIInfo _apiInfo;
        private PageInfo _pageInfo;
        private static List<PageInfo> pages = new List<PageInfo>();
        private string currentURL;
        private HashSet<string> urlSet = new HashSet<string>();


        private static AndroidDriver<AppiumWebElement> _appDriver;

        private WebDriverWait _wait;
        private int StepID = 1;
        private string TestCaseID;
        private static string TestCaseTitle;
        private static string TestresultsFolderName;
        private static string TestRunName;
        private static readonly string FrameworkMode = CommonUtil.getFrameworkConfig("framework_mode");
        private static readonly string ScreenshotMode = CommonUtil.getFrameworkConfig("screenshot_mode");
        private static readonly string TestRunFolder = CommonUtil.getFrameworkConfig("testrun_foldername");
        private static readonly string executionMode = CommonUtil.getFrameworkConfig("execution_mode");
        private static readonly string OutputFolder = CommonUtil.getFrameworkConfig("testresults_output");
        private static readonly int WebDriverWait = Int32.Parse(CommonUtil.getFrameworkConfig("webdriver_explicit_wait"));
        private static readonly string ZapMode = CommonUtil.getFrameworkConfig("ZAP_mode");
        private static readonly string PageLoadMode = CommonUtil.getFrameworkConfig("PageLoad_Mode");
        private string _zap_target;
        private static string AccessibilityTestresultsFolderName;
        private static readonly string AccessibilityOutputFolder = CommonUtil.getFrameworkConfig("accessibility_output");
        private static readonly string AccessibilityReportFolder = CommonUtil.getFrameworkConfig("accessibilityReport_foldername");
        private static readonly string AccessibilityTestMode = CommonUtil.getFrameworkConfig("accessibilityTest_Mode");

        public BaseSteps(IObjectContainer container)
        {
            _objectContainer = container;
        }

        [BeforeTestRun]

        public static void BeforeTestRun()
        {

            switch (FrameworkMode)
            {
                case "Web":
                case "Web&API":
                    if (executionMode == "Local")
                    {
                        TestRunName = TestRunFolder + FrameworkMode + "_" + executionMode+ "_" + CommonUtil.getFrameworkConfig("browser") + "_" + CommonUtil.GetDateTimeTicks();
                    }
                    else if (executionMode == "Cloud_Web")
                    {
                        TestRunName = TestRunFolder + FrameworkMode + "_" + executionMode + "_" + CommonUtil.getFrameworkConfig("os") + "_" + CommonUtil.getFrameworkConfig("os_version") + "_" +
                            CommonUtil.getFrameworkConfig("browser") + "_" + CommonUtil.getFrameworkConfig("browser_version") + "_" + CommonUtil.GetDateTimeTicks();
                    }
                    else if (executionMode == "Cloud_Mobile")
                    {
                        TestRunName = TestRunFolder + FrameworkMode + "_" + executionMode + "_" + CommonUtil.getFrameworkConfig("device") + "_" + CommonUtil.getFrameworkConfig("device_os_version") + "_" +
                            CommonUtil.getFrameworkConfig("browser") + "_" + CommonUtil.getFrameworkConfig("browser_version") + "_" + CommonUtil.GetDateTimeTicks();
                    }
                    break;
                case "App":
                    TestRunName = TestRunFolder  + "_" + CommonUtil.getFrameworkConfig("appPlatformName") + "_" + CommonUtil.getFrameworkConfig("appPlatformVersion") 
                            + "_" + CommonUtil.GetDateTimeTicks();
                    break;
                case "API":
                    TestRunName = TestRunFolder + FrameworkMode + "_" + CommonUtil.GetDateTimeTicks();
                    break;
                default:
                    Console.WriteLine("Please update FrameworkConfig.json with correct Framework Mode, possible values are Web, App, Web&API & API");
                    break;
            }

 

            //TestRunName = TestRunFolder + CommonUtil.GetDateTimeTicks();

            TestresultsFolderName = System.IO.Path.GetFullPath(OutputFolder).ToString() + TestRunName;
            System.IO.Directory.CreateDirectory(TestresultsFolderName);
            System.IO.Directory.CreateDirectory(OutputFolder + TestRunName + "/Screenshots");
            System.IO.Directory.CreateDirectory(OutputFolder + "/Downloads");

            var htmlReporter = new ExtentHtmlReporter(TestresultsFolderName + "/");
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            Reporter.AttachReporter(htmlReporter);
            string css = "img.r-img {width: 100%;}";
            htmlReporter.Config.CSS = css;

            if (ZapMode == "active"|| ZapMode == "passive")
            {
                ZapUtil.StartZAPDaemon();

            }
            else if (ZapMode == "OFF")
            {

            }

            if (AccessibilityTestMode == "ON")
            {
                AccessibilityTestresultsFolderName = System.IO.Path.GetFullPath(AccessibilityOutputFolder).ToString() + AccessibilityReportFolder + CommonUtil.GetDateTimeTicks();
                System.IO.Directory.CreateDirectory(AccessibilityTestresultsFolderName);
            }

        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            string[] filePaths = Directory.GetFiles(@OutputFolder + "/Downloads/");
            foreach (string filePath in filePaths)
                File.Delete(filePath);
            System.IO.Directory.Delete(OutputFolder + "/Downloads");

            Reporter.Flush();

          
            if (ZapMode == "active" || ZapMode == "passive")
            {
                string reportFileName = TestresultsFolderName + "/" + "ZapReport";
                ZapUtil.WriteHtmlReport(reportFileName);
                ZapUtil.ShutdownZAP();
            }
            else if (ZapMode == "OFF")
            {

            }

            if (PageLoadMode == "ON")
            {
                var output = CommonUtil.GeneratePageLoadTimeReport<PageInfo>(pages);
                File.WriteAllText(TestresultsFolderName + "/PageLoadTime.csv", output);
            }

        }


      

        [BeforeFeature]
        public static void BeforeFeature()
        {
            FeatureName = Reporter.CreateTest<Feature>(FeatureContext.Current.FeatureInfo.Title);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            switch (FrameworkMode)
            {
                case "Web":
                case "Web&API":
                    _driver = WebDriverUtil.WebInit();
                    _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(WebDriverWait));
                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    _objectContainer.RegisterInstanceAs<WebDriverWait>(_wait);
                    break;
                case "App":
                    _appDriver = MobileUtil.MobileInit();
                    _wait = new WebDriverWait(_appDriver, TimeSpan.FromSeconds(WebDriverWait));
                    _objectContainer.RegisterInstanceAs<AndroidDriver<AppiumWebElement>>(_appDriver);
                    _objectContainer.RegisterInstanceAs<WebDriverWait>(_wait);
                    break;
                case "API":
                    break;
                default:
                    Console.WriteLine("Please update FrameworkConfig.json with correct Framework Mode, possible values are Web, App, Web&API & API");
                    break;
            }
            TestCaseID = ScenarioContext.Current.ScenarioInfo.Tags[0];
            TestCaseTitle = ScenarioContext.Current.ScenarioInfo.Title;
            ScenarioName = FeatureName.CreateNode<Scenario>(TestCaseID + "-" + TestCaseTitle);

        }


        [AfterScenario]
        public void AfterScenario()
        {
            string APIInfo = JsonConvert.SerializeObject(_apiInfo);
            ScenarioName.CreateNode<Scenario>(APIInfo);
            // ScenarioName.CreateNode<Scenario>(_driver.Url);

            switch (FrameworkMode)
            {
                case "Web":
                case "Web&API":
                    WebDriverUtil.WebTearDown();
                    break;
                case "App":
                    MobileUtil.AppTearDown();
                    break;
                case "API":
                    break;
                default:
                    Console.WriteLine("Please update FrameworkConfig.json with correct Framework Mode, possible values are Web, App, Web&API & API");
                    break;
            }

        }


        [BeforeStep]
        public void BeforeStep()
        {


        }

        [AfterStep]
        public void AfterStep()
        {
            if (AccessibilityTestMode == "ON")
            {
                currentURL = _driver.Url;

                if (!urlSet.Contains(currentURL))
                {
                    var axeBuilder = new AxeBuilder(_driver);
                    var result = axeBuilder.Analyze();
                    urlSet.Add(currentURL);
                    string path = Path.Combine(AccessibilityTestresultsFolderName, _driver.Title, ".html");
                    _driver.CreateAxeHtmlReport(result, path);
                }

            }

            if (PageLoadMode == "ON")
            {

                PageInfo pageInfo = WebDriverUtil.getPageInfo(_driver);
                pageInfo.name = _driver.Url;
                pages.Add(pageInfo);

            }
            if (ScenarioContext.Current.StepContext.Keys.Contains("APIInfo"))
            {
                _apiInfo = ScenarioContext.Current.StepContext.Get<APIInfo>("APIInfo");
                _zap_target = _apiInfo.RequestInfo.Uri;
            }
            else if (ScenarioContext.Current.StepContext.Keys.Contains("PageInfo"))
            {
                _pageInfo = WebDriverUtil.getPageInfo(_driver);
                _zap_target = _pageInfo.name;
            }
            else
                _zap_target = null;


            if (ZapMode == "active")
            {
                if (_zap_target != null)
                {
                    string spiderScanId = ZapUtil.StartSpidering(_zap_target);
                    ZapUtil.PollTheSpiderTillCompletion(spiderScanId);

                    //StartAjaxSpidering();
                    //PollTheAjaxSpiderTillCompletion();

                    string activeScanId = ZapUtil.StartActiveScanning(_zap_target);
                    ZapUtil.PollTheActiveScannerTillCompletion(activeScanId);

                    ZapUtil.StartActiveScanning(_zap_target);
                }
            }
            else if (ZapMode == "passive")
            {
                if (_zap_target != null)
                {
                    string spiderScanId = ZapUtil.StartSpidering(_zap_target);
                    ZapUtil.PollTheSpiderTillCompletion(spiderScanId);

                    ZapUtil.StartPassiveScanning();
                }
            }
            else if (ZapMode == "OFF")
            {

            }


            var stackTrace = new System.Diagnostics.StackTrace();
            var stackFrames = stackTrace.GetFrames();
            bool isBackground = stackFrames.Where(x => x.GetMethod().Name == "FeatureBackground").Any();
            if (!isBackground)
            {
                var StepType = ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();
                var fileName = "/Screenshots/" + ScenarioContext.Current.ScenarioInfo.Tags[0] + "." + StepID.ToString() + ".png";
                if (ScenarioContext.Current.TestError != null)
                {
                    var FailureMessage = "Step Failed: " + ScenarioContext.Current.TestError.Message + "\r\n"
                              + "Stractrace :" + ScenarioContext.Current.TestError.StackTrace;
                    if (FrameworkMode == "Web" || FrameworkMode == "Web&API")
                    {
                        WebDriverUtil.SaveScreenShot(TestresultsFolderName + fileName);

                        if (StepType == "Given")
                            ScenarioName.CreateNode<Given>("Given " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage, MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());
                        else if (StepType == "When")
                            ScenarioName.CreateNode<When>("When " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage, MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());
                        else if (StepType == "Then")
                            ScenarioName.CreateNode<Then>("Then " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage, MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());
                        else if (StepType == "And")
                            ScenarioName.CreateNode<Then>("And " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage, MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());

                    }
                    else
                    {
                        if (StepType == "Given")
                            ScenarioName.CreateNode<Given>("Given " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage);
                        else if (StepType == "When")
                            ScenarioName.CreateNode<When>("When " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage);
                        else if (StepType == "Then")
                            ScenarioName.CreateNode<Then>("Then " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage);
                        else if (StepType == "And")
                            ScenarioName.CreateNode<Then>("And " + ScenarioStepContext.Current.StepInfo.Text).Fail(FailureMessage);
                    }
                }
                else
                {
                    if (ScenarioContext.Current.ScenarioExecutionStatus.ToString() == "OK")
                    {
                        if ((FrameworkMode == "Web" || FrameworkMode == "Web&API") && ScreenshotMode == "Pass/Fail")
                        {
                            WebDriverUtil.SaveScreenShot(TestresultsFolderName + fileName);
                            if (StepType == "Given")
                                ScenarioName.CreateNode<Given>("Given " + ScenarioStepContext.Current.StepInfo.Text).Pass("", MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());
                            else if (StepType == "When")
                                ScenarioName.CreateNode<When>("When " + ScenarioStepContext.Current.StepInfo.Text).Pass("", MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());
                            else if (StepType == "Then")
                                ScenarioName.CreateNode<Then>("Then " + ScenarioStepContext.Current.StepInfo.Text).Pass("", MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());
                            else if (StepType == "And")
                                ScenarioName.CreateNode<Then>("And " + ScenarioStepContext.Current.StepInfo.Text).Pass("", MediaEntityBuilder.CreateScreenCaptureFromPath("." + fileName).Build());

                        }
                        else
                        {
                            if (StepType == "Given")
                                ScenarioName.CreateNode<Given>("Given " + ScenarioStepContext.Current.StepInfo.Text).Pass("");
                            else if (StepType == "When")
                                ScenarioName.CreateNode<When>("When " + ScenarioStepContext.Current.StepInfo.Text).Pass("");
                            else if (StepType == "Then")
                                ScenarioName.CreateNode<Then>("Then " + ScenarioStepContext.Current.StepInfo.Text).Pass("");
                            else if (StepType == "And")
                                ScenarioName.CreateNode<Then>("And " + ScenarioStepContext.Current.StepInfo.Text).Pass("");
                        }
                    }
                    else
                    {
                        if (StepType == "Given")
                            ScenarioName.CreateNode<Given>("Given " + ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                        else if (StepType == "When")
                            ScenarioName.CreateNode<When>("When " + ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                        else if (StepType == "Then")
                            ScenarioName.CreateNode<Then>("Then " + ScenarioStepContext.Current.StepInfo.Text).Skip("Step Definition Pending");
                    }
                }
            }

            StepID += 1;



        }


    }
}
