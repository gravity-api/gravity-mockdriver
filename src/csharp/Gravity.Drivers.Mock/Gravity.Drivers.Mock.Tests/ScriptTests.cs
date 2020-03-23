using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Mock;
using OpenQA.Selenium.Support.UI;
using System;

#pragma warning disable S4144
namespace Gravity.Drivers.Mock.Tests
{
    // demo comment
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

        private void WaitForState(string state) => wait.Until(driver =>
        {
            var scriptResult = (string)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState;");
            return scriptResult == state;
        });

        [DataTestMethod]
        [DataRow("window.scroll(top: 500, left: 400, behavior: auto)")]
        public void ScrollWindow(string script)
        {
            // setup
            var driver = new MockWebDriver();

            // execute
            driver.ExecuteScript(script);

            // assert
            Assert.IsTrue(driver.Manage().Window.Position.Y == 500);
            Assert.IsTrue(driver.Manage().Window.Position.X == 400);
        }

        [DataTestMethod]
        [DataRow("arguments[0].scroll(top: 500, left: 400, behavior: auto)")]
        public void ScrollElement(string script)
        {
            // setup
            var driver = new MockWebDriver();
            var element = driver.FindElement(MockBy.Positive());

            // execute
            driver.ExecuteScript(script, element);

            // actuals
            var scrollTop = (string)driver.ExecuteScript("return arguments[0].scrollTop;", element);
            var scrollLeft = (string)driver.ExecuteScript("return arguments[0].scrollLeft;", element);

            // assert
            Assert.IsTrue(scrollLeft == "400");
            Assert.IsTrue(scrollTop == "500");
        }

        [TestMethod]
        public void ScriptElementPositive()
        {
            // setup
            var driver = new MockWebDriver();

            // execute
            driver.ExecuteScript("arguments[0].checked=false");

            // assert
            Assert.IsTrue(true);
        }

        [DataTestMethod]
        [DataRow("window.open('about:blank', '_blank');")]
        public void NewTab(string script)
        {
            // setup
            var driver = new MockWebDriver();
            var expected = driver.WindowHandles.Count + 1;

            // execute
            driver.ExecuteScript(script);

            // assert
            Assert.AreEqual(expected, actual: driver.WindowHandles.Count);
        }
    }
}
#pragma warning restore S4144