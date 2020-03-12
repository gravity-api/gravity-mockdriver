using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Mock;

namespace Gravity.Drivers.Mock.Tests
{
    [TestClass]
    public class GoToUrlTests
    {
        [TestMethod]
        public void GoToUrlPositive()
        {
            // setup
            var driver = new MockWebDriver();

            // navigate
            driver.Navigate().GoToUrl("http://noaddress.io");

            // navigate
            Assert.AreEqual("http://noaddress.io", driver.Url);
        }
    }
}
