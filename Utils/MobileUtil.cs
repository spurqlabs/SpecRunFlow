using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Service;
using System;

namespace SpurFlow.Utils
{
    public static class MobileUtil
    {
        private static AppiumLocalService _appiumLocalService;
        private static AndroidDriver<AppiumWebElement> _appDriver;
       
        public static AndroidDriver<AppiumWebElement> MobileInit()
        {
            var appiumBuilder = new AppiumServiceBuilder();
            _appiumLocalService = appiumBuilder.Build();
            _appiumLocalService.Start();
            var appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability(MobileCapabilityType.DeviceName, CommonUtil.getFrameworkConfig("appDeviceName"));
            appiumOptions.AddAdditionalCapability(MobileCapabilityType.PlatformName, CommonUtil.getFrameworkConfig("appPlatformName"));
            appiumOptions.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, CommonUtil.getFrameworkConfig("appPlatformVersion"));
            appiumOptions.AddAdditionalCapability(MobileCapabilityType.Udid, CommonUtil.getFrameworkConfig("AppUdid"));
            //http://www.automationtestinghub.com/apppackage-and-appactivity-name/
            appiumOptions.AddAdditionalCapability("appPackage", CommonUtil.getFrameworkConfig("appPackage"));
            appiumOptions.AddAdditionalCapability("appActivity", CommonUtil.getFrameworkConfig("appActivity"));
            appiumOptions.AddAdditionalCapability(MobileCapabilityType.NoReset, CommonUtil.getFrameworkConfig("appNoReset"));
            appiumOptions.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, CommonUtil.getFrameworkConfig("appNewCommandTimeout"));
            try
            {
          //      _appDriver = new AndroidDriver<AppiumWebElement>(_appiumLocalService, appiumOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }

            return _appDriver;
        }

        public static void AppTearDown()
        {
            _appDriver.CloseApp();

        }
    }

}
