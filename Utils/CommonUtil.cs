using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;

namespace SpurFlow.Utils
{
    public static class CommonUtil
    {
        public static string GetDateTimeTicks()
        {
            long time = DateTime.Now.Ticks;
            return time.ToString();
        }

        public static string getFrameworkConfig(string parameter)
        {
            string configStream = File.ReadAllText(@"../FrameworkConfig.json");
            var config = JObject.Parse(configStream);
            string configValue = config.GetValue(parameter).ToString();
            return configValue;
        }

        public static string getAppConfig(string parameter)
        {
            string configStream = File.ReadAllText(@"../AppConfig.json");
            var config = JObject.Parse(configStream.ToString());
            string configValue = config.GetValue(parameter).ToString();
            return configValue;
        }


        public static string getTimezone()
        {
            var zone = TimeZoneInfo.Local;
            return zone.ToString();

        }

        public static string GetTimestamp()
        {
            DateTime now = DateTime.Now;
            string mDate = now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            return mDate;
        }

        public static string Getfilename(string Name)
        {

            string filename = Name + "_" + GetDateTimeTicks() + ".png";

            return filename;
        }

        public static string getLatestFilefromDir(String dirPath)
        {

            string[] filePaths = Directory.GetFiles(dirPath);


            FileInfo LastFileInfo = new FileInfo(filePaths[0]);
            for (int i = 1; i < filePaths.Length; i++)
            {
                FileInfo FileInfo = new FileInfo(filePaths[i]);

                if (LastFileInfo.LastWriteTime < FileInfo.LastWriteTime)
                {
                    LastFileInfo = FileInfo;
                }
            }
            return LastFileInfo.Name;
        }

        public static List<Dictionary<string, string>> ConvertToDictionaryList(this Table dt)
        {
            var lstDict = new List<Dictionary<string, string>>();

            if (dt != null)
            {
                var headers = dt.Header;

                foreach (var row in dt.Rows)
                {
                    var dict = new Dictionary<string, string>();
                    foreach (var header in headers)
                    {
                        dict.Add(header, row[header]);
                    }

                    lstDict.Add(dict);
                }

            }
            return lstDict;

        }
        public static string GetCurrentPSTDate()
        {
            var utc = DateTime.UtcNow;
            TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var pacificTime = TimeZoneInfo.ConvertTimeFromUtc(utc, pacificZone);
            string pstDate = pacificTime.ToString("MMMM d, yyyy");
            return pstDate.ToString();

        }

        public static string GeneratePageLoadTimeReport<T>(List<T> items) where T : class
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
