using System;

namespace OpenQA.Selenium.Mock
{
    /// <summary>
    /// Defines an interface allowing the user to access the browser's history and to
    /// navigate to a given URL.
    /// </summary>
    /// <seealso cref="INavigation" />
    public class MockNavigation : INavigation
    {
        // members: state
        private readonly IWebDriver driver;

        /// <summary>
        /// Creates a new instance of <see cref="MockNavigation"/>.
        /// </summary>
        /// <param name="driver"><see cref="MockWebDriver"/> on which this <see cref="MockNavigation"/> based.</param>
        public MockNavigation(MockWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Move back a single entry in the browser's history.
        /// </summary>
        public void Back()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Move a single "item" forward in the browser's history.
        /// </summary>
        /// <remarks>
        /// Does nothing if we are on the latest page viewed.
        /// </remarks>
        public void Forward()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load. It is best to use a fully qualified URL.</param>
        public void GoToUrl(string url)
        {
            driver.Url = url;
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        public void GoToUrl(Uri url)
        {
            driver.Url = url.AbsoluteUri;
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public void Refresh()
        {
            // Method intentionally left empty.
        }
    }
}