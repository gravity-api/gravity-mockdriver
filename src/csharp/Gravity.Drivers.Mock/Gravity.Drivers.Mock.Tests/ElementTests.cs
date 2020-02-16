using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Mock;
using System;

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
            Assert.IsTrue(iterations > 1);
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