using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Mock;
using System;

#pragma warning disable S2699
namespace Gravity.Drivers.Mock.Tests
{
    [TestClass]
    public class GoToUrlTests
    {
        [DataTestMethod]
        [DataRow("http://m.positive.io")]
        [DataRow("http://m.default.io")]
        public void GoToUrlPropertyPositive(string url)
        {
            DoGoToUrl(url, isProperty: true);
        }

        [DataTestMethod]
        [DataRow("http://m.exception.io"), ExpectedException(typeof(WebDriverException))]
        public void GoToUrlPropertyException(string url)
        {
            DoGoToUrl(url, isProperty: true);
        }

        [DataTestMethod, ExpectedException(typeof(ArgumentNullException))]
        [DataRow("http://m.none.io")]
        [DataRow("http://m.null.io")]
        public void GoToUrlPropertyNullOrNone(string url)
        {
            DoGoToUrl(url, isProperty: true);
        }

        [DataTestMethod]
        [DataRow("http://m.positive.io")]
        [DataRow("http://m.default.io")]
        public void GoToUrlPositive(string url)
        {
            DoGoToUrl(url, isProperty: false);
        }

        [DataTestMethod]
        [DataRow("http://m.exception.io"), ExpectedException(typeof(WebDriverException))]
        public void GoToUrlException(string url)
        {
            DoGoToUrl(url, isProperty: false);
        }

        [DataTestMethod, ExpectedException(typeof(ArgumentNullException))]
        [DataRow("http://m.none.io")]
        [DataRow("http://m.null.io")]
        public void GoToUrlNullOrNone(string url)
        {
            DoGoToUrl(url, isProperty: false);
        }

        private void DoGoToUrl(string url, bool isProperty)
        {
            // setup
            var driver = new MockWebDriver();

            // navigate
            if (isProperty)
            {
                driver.Url = url;
            }
            else
            {
                driver.Navigate().GoToUrl(url);
            }

            // assertion
            Assert.AreEqual(expected: url, actual: driver.Url);
        }
    }
}
#pragma warning restore
