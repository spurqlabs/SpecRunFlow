using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpurFlow.Pages
{
    public class CalculatorPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;
        public CalculatorPage(IWebDriver _driver, WebDriverWait wait)
        {
            this.driver = _driver;
            this.wait = wait;

        }

        public void set_url(string url)
        {
            //driver.Navigate().GoToUrl(url);

        }

        public void enter_number(String n)
        {
            IWebElement number_element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//span[@onclick='r(" + n + ")']")));
            number_element.Click();
        }

        public void enter_operator(String op)
        {
            IWebElement operator_element = driver.FindElement(By.XPath("//span[@onclick=\"r('" + op + "')\"]"));
            operator_element.Click();
        }

        public String getResult()
        {
            IWebElement result = driver.FindElement(By.Id("sciOutPut"));
            String actual_result = result.Text;
            actual_result = actual_result.Trim();
            return actual_result;

        }


    }
}

