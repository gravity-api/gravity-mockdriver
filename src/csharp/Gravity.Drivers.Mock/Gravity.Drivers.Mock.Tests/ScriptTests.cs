using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Mock;
using OpenQA.Selenium.Support.UI;
using System;

#pragma warning disable S4144
namespace Gravity.Drivers.Mock.Tests
{
    [TestClass]
    public class ScriptTests
    {
        // members: state
        private readonly WebDriverWait wait;

        public ScriptTests()
        {
            wait = new WebDriverWait(new MockWebDriver(), TimeSpan.FromSeconds(5));
        }

        [DataTestMethod]
        [DataRow("uninitialized")]
        [DataRow("loading")]
        [DataRow("loaded")]
        [DataRow("interactive")]
        [DataRow("complete")]
        public void ReadyStateScript(string expectedState)
        {
            // setup
            wait.Timeout = TimeSpan.FromSeconds(180);

            // execute script until
            WaitForState(state: expectedState);

            // assert for true result (pass if no timeout exception)
            Assert.IsTrue(true);
        }

        [DataTestMethod, ExpectedException(typeof(WebDriverTimeoutException))]
        [DataRow("not_a_state")]
        public void ReadyStateScriptTimeout(string expectedState)
        {
            // setup
            wait.Timeout = TimeSpan.FromSeconds(1);

            // execute script until
            WaitForState(state: expectedState);

            // assert for true result
            Assert.IsTrue(true);
        }

        private void WaitForState(string state)=> wait.Until(driver =>
        {
            var scriptResult = (string)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState;");
            return scriptResult == state;
        });
    }
}
#pragma warning restore S4144