using OWASPZAPDotNetAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Xunit;

namespace SpurFlow.Utils
{
    public static class ZapUtil
    {
        public static string _target = CommonUtil.getAppConfig("ZAP_app_URL");
        public static ClientApi _api = new ClientApi(CommonUtil.getFrameworkConfig("ZAP_host"), Int32.Parse(CommonUtil.getFrameworkConfig("ZAP_port")), CommonUtil.getFrameworkConfig("ZAP_API_KEY"));
        public static IApiResponse _apiResponse;

        public static void StartZapUI()
        {
            Console.WriteLine("Trying to StartZapUI");
            ProcessStartInfo zapProcessStartInfo = new ProcessStartInfo();
            zapProcessStartInfo.FileName = @"C:\Program Files\OWASP\Zed Attack Proxy\ZAP.exe";
            zapProcessStartInfo.WorkingDirectory = @"C:\Program Files\OWASP\Zed Attack Proxy";

            Console.WriteLine(zapProcessStartInfo.ToString());
            Console.WriteLine("Issuing command to StartZapUI");
            Process zap = Process.Start(zapProcessStartInfo);

            //Sleep(120000);
            CheckIfZAPHasStartedByPollingTheAPI(1);
        }

        public static void StartZAPDaemon()
        {
            Console.WriteLine("Trying to StartZAPDaemon");
            ProcessStartInfo zapProcessStartInfo = new ProcessStartInfo();
            zapProcessStartInfo.FileName = @"C:\Program Files\OWASP\Zed Attack Proxy\ZAP.exe";
            zapProcessStartInfo.WorkingDirectory = @"C:\Program Files\OWASP\Zed Attack Proxy";
            zapProcessStartInfo.Arguments = "-daemon -host 127.0.0.1 -port 9090";

            Console.WriteLine("Issuing command to StartZAPDaemon");
            Console.WriteLine(zapProcessStartInfo.ToString());
            Process zap = Process.Start(zapProcessStartInfo);

            Sleep(5000);
            CheckIfZAPHasStartedByPollingTheAPI(1);
        }

        public static void Sleep(int sleepTime)
        {
            Console.WriteLine("Sleeping for {0} minutes", sleepTime / 1000);
            Thread.Sleep(sleepTime);
        }

        public static void CheckIfZAPHasStartedByPollingTheAPI(int minutesToWait)
        {
            WebClient webClient = new WebClient();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int millisecondsToWait = minutesToWait * 60 * 1000;
            string zapUrlToDownload = "http://localhost:9090";

            while (millisecondsToWait > watch.ElapsedMilliseconds)
            {
                try
                {
                    Console.WriteLine("Trying to check if ZAP has started by accessing the ZAP API at {0}", zapUrlToDownload);
                    string responseString = webClient.DownloadString(zapUrlToDownload);
                    Console.WriteLine(Environment.NewLine + responseString + Environment.NewLine);
                    Console.WriteLine("Obtained a response from the ZAP API at {0} {1}Hence assuming that ZAP started successfully", zapUrlToDownload, Environment.NewLine);
                    return;
                }
                catch (WebException webException)
                {
                    Console.WriteLine("Seems like ZAP did not start yet");
                    Console.WriteLine(webException.Message + Environment.NewLine);
                    Console.WriteLine("Sleeping for 2 seconds");
                    Thread.Sleep(2000);
                }
            }

            throw new Exception(string.Format("Waited for {0} minute(s), however could not access the API successfully, hence could not verify if ZAP started successfully or not", minutesToWait));
        }


        [Fact]
        public static void startactivetest()
        {

            string spiderScanId = StartSpidering(_target);
            PollTheSpiderTillCompletion(spiderScanId);

            //StartAjaxSpidering();
            //PollTheAjaxSpiderTillCompletion();

            string activeScanId = StartActiveScanning(_target);
            PollTheActiveScannerTillCompletion(activeScanId);

            string reportFileName = string.Format("report-{0}", DateTime.Now.ToString("dd-MMM-yyyy-hh-mm-ss"));
            WriteXmlReport(reportFileName);
            WriteHtmlReport(reportFileName);
            PrintAlertsToConsole();

            ShutdownZAP();

        }


        [Fact]
        public static void startpassivetest()
        {
            ZapUtil.StartZAPDaemon();

            string spiderScanId = StartSpidering(_target);
            PollTheSpiderTillCompletion(spiderScanId);

            StartPassiveScanning();

            string reportFileName = string.Format("report-{0}", DateTime.Now.ToString("dd-MMM-yyyy-hh-mm-ss"));
            WriteXmlReport(reportFileName);
            WriteHtmlReport(reportFileName);
            PrintAlertsToConsole();

            ShutdownZAP();

        }
        public static void Go()
        {
            string spiderScanId = StartSpidering(_target);
            PollTheSpiderTillCompletion(spiderScanId);

            //StartAjaxSpidering();
           // PollTheAjaxSpiderTillCompletion();

            string activeScanId = StartActiveScanning(_target);
            PollTheActiveScannerTillCompletion(activeScanId);

            string reportFileName = string.Format("report-{0}", DateTime.Now.ToString("dd-MMM-yyyy-hh-mm-ss"));
            WriteXmlReport(reportFileName);
            WriteHtmlReport(reportFileName);
            PrintAlertsToConsole();

            ShutdownZAP();
        }

        public static void ShutdownZAP()
        {
            _apiResponse = _api.core.shutdown();
            if ("OK" == ((ApiResponseElement)_apiResponse).Value)
                Console.WriteLine("ZAP shutdown success " + _target);
        }

        public static void PrintAlertsToConsole()
        {
            List<Alert> alerts = _api.GetAlerts(_target, 0, 0, string.Empty);
            foreach (var alert in alerts)
            {
                Console.WriteLine(alert.AlertMessage
                    + Environment.NewLine
                    + alert.CWEId
                    + Environment.NewLine
                    + alert.Url
                    + Environment.NewLine
                    + alert.WASCId
                    + Environment.NewLine
                    + alert.Evidence
                    + Environment.NewLine
                    + alert.Parameter
                    + Environment.NewLine
                );
            }
        }

        public static void WriteHtmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".html", _api.core.htmlreport());
        }

        public static void WriteXmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".xml", _api.core.xmlreport());
        }

        public static void PollTheActiveScannerTillCompletion(string activeScanId)
        {
            int activeScannerprogress;
            while (true)
            {
                Sleep(5000);
                activeScannerprogress = int.Parse(((ApiResponseElement)_api.ascan.status(activeScanId)).Value);
                Console.WriteLine("Active scanner progress: {0}%", activeScannerprogress);
                if (activeScannerprogress >= 100)
                    break;
            }
            Console.WriteLine("Active scanner complete");
        }

        public static string StartActiveScanning(string _target)
        {
            Console.WriteLine("Active Scanner: " + _target);
            _apiResponse = _api.ascan.scan(_target, "", "", "", "", "", "");

            string activeScanId = ((ApiResponseElement)_apiResponse).Value;
            return activeScanId;
        }

        public static void StartPassiveScanning()
        {
            int numberOfRecords;

            try
            {
                // TODO : explore the app (Spider, etc) before using the Passive Scan API, Refer the explore section for details
                // Loop until the passive scan has finished
                while (true)
                {
                    Thread.Sleep(2000);
                    _apiResponse = _api.pscan.recordsToScan();
                    numberOfRecords = Int32.Parse(((ApiResponseElement)_api.pscan.recordsToScan()).Value);
                    Console.WriteLine("Number of records left for scanning : " + numberOfRecords);
                    if (numberOfRecords == 0)
                    {
                        break;
                    }
                }
                Console.WriteLine("Passive Scan completed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message + e.StackTrace);

            }
        }

        public static void PollTheAjaxSpiderTillCompletion()
        {
            while (true)
            {
                Sleep(1000);
                string ajaxSpiderStatusText = string.Empty;
                ajaxSpiderStatusText = Convert.ToString(((ApiResponseElement)_api.ajaxspider.status()).Value);
                if (ajaxSpiderStatusText.IndexOf("running", StringComparison.InvariantCultureIgnoreCase) > -1)
                    Console.WriteLine("Ajax Spider running");
                else
                    break;
            }

            Console.WriteLine("Ajax Spider complete");
            Sleep(10000);
        }

        public static string StartAjaxSpidering(string _target)
        {
            Console.WriteLine("Ajax Spider: " + _target);
            _apiResponse = _api.ajaxspider.scan(_target, "", "", "");

            string scanid = ((ApiResponseElement)_apiResponse).Value;
            return scanid;
        }

        public static void PollTheSpiderTillCompletion(string scanid)
        {
            int spiderProgress;
            while (true)
            {
                Sleep(1000);
                spiderProgress = int.Parse(((ApiResponseElement)_api.spider.status(scanid)).Value);
                Console.WriteLine("Spider progress: {0}%", spiderProgress);
                if (spiderProgress >= 100)
                    break;
            }

            Console.WriteLine("Spider complete");
            Sleep(10000);
        }

        public static string StartSpidering(string _target)
        {
            Console.WriteLine("Spider: " + _target);
            _apiResponse = _api.spider.scan(_target, "", "", "", "");
            string scanid = ((ApiResponseElement)_apiResponse).Value;
            return scanid;
        }

        public static void LoadTargetUrlToSitesTree()
        {
            _api.AccessUrl(_target);
        }

       
    }
}
