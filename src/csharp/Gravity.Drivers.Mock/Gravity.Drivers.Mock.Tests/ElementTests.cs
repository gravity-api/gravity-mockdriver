using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Mock;
using System;
using System.Text.RegularExpressions;

namespace Gravity.Drivers.Mock.Tests
{
    [TestClass]
    public class ElementTests
    {
        [TestMethod]
        public void FileElement()
        {
            // generate element
            var onElement = MockWebElement.GetElement(new MockWebDriver(), MockBy.InputFile());

            // assertion
            Assert.IsTrue(onElement.TagName == "INPUT");
            Assert.IsTrue(onElement.GetAttribute("type") == "file");
        }

        [TestMethod]
        public void RandomNull() => Execute(attempts: 5, test: () =>
        {
            // setup
            var iterations = 0;

            // generate element
            var onElement = MockWebElement.GetElement(new MockWebDriver(), MockBy.Positive());

            // iterate
            while (onElement != null)
            {
                iterations++;
                onElement = MockWebElement.GetElement(new MockWebDriver(), MockBy.RandomNull());
            }

            // assertion
            Assert.IsTrue(iterations > 0);
        });

        [TestMethod]
        public void RandomNoSuchElement() => Execute(attempts: 5, test: () =>
        {
            // setup
            var iterations = 0;

            // iterate
            while (iterations < 1)
            {
                try
                {
                    MockWebElement.GetElement(new MockWebDriver(), MockBy.RandomNoSuchElement());
                }
                catch (Exception e) when (e is NoSuchElementException)
                {
                    iterations++;
                }
            }

            // assertion
            Assert.IsTrue(iterations > 0);
        });

        [TestMethod]
        public void RandomStale() => Execute(attempts: 5, test: () =>
        {
            // setup
            var iterations = 0;

            // iterate
            while (iterations < 1)
            {
                try
                {
                    MockWebElement.GetElement(new MockWebDriver(), MockBy.RandomStale());
                }
                catch (Exception e) when (e is StaleElementReferenceException)
                {
                    iterations++;
                }
            }

            // assertion
            Assert.IsTrue(iterations > 0);
        });

        [DataTestMethod]
        [DataRow("href", "http://m.from-href.io/")]
        [DataRow("index", "0")]
        [DataRow("any", "mock attribute value")]
        public void Attributes(string attribute, string expected)
        {
            // actual
            var actual = new MockWebDriver().FindElement(MockBy.Positive()).GetAttribute(attribute);

            // assertion
            Assert.IsTrue(Regex.IsMatch(actual, expected));
        }

        [DataTestMethod]
        [DataRow("exception"), ExpectedException(typeof(WebDriverException))]
        public void AttributesException(string attribute)
        {
            // actual
            new MockWebDriver().FindElement(MockBy.Positive()).GetAttribute(attribute);

            // assertion
            Assert.IsTrue(false);
        }

        [DataTestMethod]
        [DataRow("null", null)]
        public void AttributesNull(string attribute, string expected)
        {
            // actual
            var actual = new MockWebDriver().FindElement(MockBy.Positive()).GetAttribute(attribute);

            // assertion
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("value", "test value")]
        public void AttributesValue(string attribute, string expected)
        {
            // setup
            var element = new MockWebDriver().FindElement(MockBy.Positive());
            element.SendKeys(expected);

            // get actual
            var actual = element.GetAttribute(attribute);

            // assertion
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("//div")]
        public void WhiteListMultiple(string locator)
        {
            // setup
            var elements = new MockWebDriver().FindElements(By.XPath(locator));

            // assertion
            Assert.IsTrue(elements.Count > 0);
        }

        [DataTestMethod]
        [DataRow("//div")]
        public void WhiteListSingle(string locator)
        {
            // setup
            var element = new MockWebDriver().FindElement(By.XPath(locator));

            // assertion
            Assert.IsTrue(element != null);
        }

        [DataTestMethod, ExpectedException(typeof(NoSuchElementException))]
        [DataRow("not a locator")]
        public void WhiteListNegative(string locator)
        {
            // setup
            new MockWebDriver().FindElement(By.XPath(locator));

            // assertion (expected exception)
            Assert.IsTrue(true);
        }

        private void Execute(int attempts, Action test)
        {
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    test.Invoke();
                    return;
                }
                catch (Exception e) when (e != null)
                {
                    if (i == attempts - 1)
                    {
                        throw;
                    }
                }
            }
        }
    }
}