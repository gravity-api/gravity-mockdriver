using OpenQA.Selenium.Mock.Extensions;
using System;
using System.ComponentModel;

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
        private readonly MockWebDriver driver;

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
            DoGoToUrl(url);
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        public void GoToUrl(Uri url)
        {
            DoGoToUrl(url: url.AbsoluteUri);
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public void Refresh()
        {
            // Method intentionally left empty.
        }

        // EXECUTE NAVIGATION FACTORY
#pragma warning disable S3400, IDE0051, IDE0022, IDE0060
        [Description("exception")]
        private void UrlException(string url) => throw new WebDriverException();

        [Description("none|null")]
        private void UrlNoneOrNull(string url)
            => throw new ArgumentNullException(nameof(url), $"Argument [{nameof(url)}] cannot be null.");

        [Description("positive")]
        private void UrlPositive(string url)
        {
            driver.CurrentUrl(currentUrl: url);
        }

        private void DoGoToUrl(string url)
        {
            // get method to execute
            var method = GetType().GetMethodByDescription(actual: url);

            // default
            if (method == default)
            {
                UrlPositive(url);
                return;
            }

            // invoke
            try
            {
                method.Invoke(this, new object[] { url });
            }
            catch (Exception e)
            {
                throw e.InnerException != default ? e.InnerException : e;
            }
        }
#pragma warning restore
    }
}