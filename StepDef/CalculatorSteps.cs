using SpurFlow.Pages;
using System;
using System.Threading;
using TechTalk.SpecFlow;
using Xunit;

namespace SpurFlow.StepDef
{
    [Binding]
    public class CalculatorSteps
    {
        private CalculatorPage calc_page;


        public CalculatorSteps(CalculatorPage calc_page)
        {

            this.calc_page = calc_page;

        }

        [Given(@"User is on Calculator page")]
        public void GivenUserIsOnCalculatorPage()
        {

            //calc_page.set_url("http://www.calculator.net");
        }

        [When(@"User enters (.*)")]
        public void WhenUserEnters(int n)
        {
            calc_page.enter_number(n.ToString());

        }

        [When(@"User press '(.*)'")]
        public void WhenUserPress(string op)
        {

            calc_page.enter_operator(op);

        }


        [Then(@"User verifies result is (.*)")]
        public void ThenUserVerifiesResultIs(int expected_result)
        {

            String actual_result = calc_page.getResult();
            Assert.Equal(expected_result.ToString(), actual_result);
            Thread.Sleep(2000);

        }

    }
}
