using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Mock;

namespace Gravity.Drivers.Mock.Tests
{
    [TestClass]
    public class PageSourceTests
    {
        [TestMethod]
        public void PageSourceSet()
        {
            // setup
            const string Html = "<html></html>";
            var driver = new MockWebDriver();

            // assertion
            Assert.AreNotEqual(notExpected: Html, actual: driver.PageSource);

            // execute
            driver.PageSource = Html;

            // assertion
            Assert.AreEqual(expected: Html, actual: driver.PageSource);
        }
    }
}
