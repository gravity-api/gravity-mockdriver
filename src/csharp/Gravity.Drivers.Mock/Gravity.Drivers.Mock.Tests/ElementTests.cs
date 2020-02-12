using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Mock;

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
    }
}
