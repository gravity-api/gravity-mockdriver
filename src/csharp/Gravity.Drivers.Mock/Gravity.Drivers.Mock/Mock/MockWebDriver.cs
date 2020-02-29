/*
 * CHANGE LOG - keep only last 5 threads
 * 
 * 2020-01-01
 *    - modify: re-factor ExecuteScript to use methods factory instead of conditions
 * 
 * on-line resources
 */
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Mock.Extensions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenQA.Selenium.Mock
{
    /// <summary>
    /// Defines the interface through which the user controls the browser.
    /// </summary>
    public class MockWebDriver : IWebDriver, IJavaScriptExecutor, IHasSessionId, IActionExecutor, IHasInputDevices, ITakesScreenshot
    {
        // members: state
        private int xPosition;
        private int yPosition;

        #region *** constructors ***
        /// <summary>
        /// Initializes a new instance of the <see cref="MockWebDriver"/> class.
        /// </summary>
        public MockWebDriver() : this(".") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWebDriver"/> class.
        /// </summary>
        /// <param name="driverBinaries">The full path to the directory containing driver executables.</param>
        public MockWebDriver(string driverBinaries)
            : this(driverBinaries, new Dictionary<string, object>())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWebDriver"/> class.
        /// </summary>
        /// <param name="driverBinaries">The full path to the directory containing driver executables.</param>
        /// <param name="capabilities">A collection containing the desired capabilities of this <see cref="MockWebDriver"/>.</param>
        public MockWebDriver(string driverBinaries, IDictionary<string, object> capabilities)
        {
            // setup
            SetChildWindows(capabilities);

            // state
            CurrentWindowHandle = WindowHandles[0];
            SessionId = new SessionId($"mock-{Guid.NewGuid()}");
            DriverBinaries = driverBinaries;
            Capabilities = capabilities;
        }
        #endregion

        #region *** properties   ***
        /// <summary>
        /// Gets or sets the URL the browser is currently displaying.
        /// </summary>
        public string Url { get; set; } = "http://mockgravityapiurl.com/";

        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        public string Title => "Mock Gravity API Page Title";

        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        public string PageSource => new StringBuilder()
            .Append("<html>")
            .Append("   <head>".Trim())
            .Append("       <body class=\"mockClass\">".Trim())
            .Append("           <div id=\"mockDiv\">mock div text</div>".Trim())
            .Append("           <positive>mock div text 1</positive>".Trim())
            .Append("           <positive>mock div text 2</positive>".Trim())
            .Append("           <positive>mock div text 3</positive>".Trim())
            .Append("       </body>".Trim())
            .Append("   </head>".Trim())
            .Append("</html>")
            .ToString();

        /// <summary>
        /// Gets the current window handle, which is an opaque handle to this window that
        /// uniquely identifies it within this driver instance.
        /// </summary>
        public string CurrentWindowHandle { get; set; }

        /// <summary>
        /// Get the current driver binaries location.
        /// </summary>
        public string DriverBinaries { get; }

        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        public ReadOnlyCollection<string> WindowHandles { get; private set; }

        /// <summary>
        /// Gets the session ID of the current session.
        /// </summary>
        public SessionId SessionId { get; }

        /// <summary>
        /// Gets a value indicating whether this object is a valid action executor. 
        /// </summary>
        public bool IsActionExecutor => true;

        /// <summary>
        /// Gets a collection containing the desired capabilities of this <see cref="MockWebDriver"/>.
        /// </summary>
        public IDictionary<string, object> Capabilities { get; }

        /// <summary>
        /// Provides methods representing basic keyboard actions.
        /// </summary>
        public IKeyboard Keyboard => new MockKeyboard();

        /// <summary>
        /// Provides methods representing basic mouse actions.
        /// </summary>
        public IMouse Mouse => new MockMouse();
        #endregion

        #region *** selenium     ***
        /// <summary>
        /// Close the current window, quitting the browser if it is the last window currently open.
        /// </summary>
        public void Close()
        {
            // exception conditions
            var isKey = Capabilities.ContainsKey(MockCapabilities.ThrowOnClose);
            var isException = isKey && (bool)Capabilities[MockCapabilities.ThrowOnClose];
            if (isException)
            {
                throw new WebDriverException();
            }

            // exit conditions
            if (WindowHandles.Count == 0)
            {
                return;
            }

            // remove current windows
            var windowHandles = WindowHandles.ToList();
            windowHandles.Remove(CurrentWindowHandle);
            WindowHandles = new ReadOnlyCollection<string>(windowHandles);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">True to perform cleanup associated tasks.</param>
        protected virtual void Dispose(bool disposing)
        {
            WindowHandles = new ReadOnlyCollection<string>(new List<string>());
        }

        /// <summary>
        /// Executes JavaScript asynchronously in the context of the currently selected frame
        /// or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteAsyncScript(string script, params object[] args) => ExecuteScript(script, args);

        /// <summary>
        /// Executes JavaScript in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteScript(string script, params object[] args)
        {
            // exit conditions
            if (args.Length > 0 && args.Any(i => i == null))
            {
                throw new WebDriverTimeoutException();
            }

            // setup binding flags
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

            // get script generating method
            var method = GetType().GetMethodByDescription(script, Flags);

            // exit conditions
            if (method == null)
            {
                throw new WebDriverException();
            }

            // return mock script
            try
            {
                var arguments = GetScriptMethodArguments(method, script, args);
                return (string)method.Invoke(this, arguments);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
                throw;
            }
        }

        private object[] GetScriptMethodArguments(MethodInfo method, string script, params object[] args)
        {
            var parametes = method.GetParameters();
            object[] arguments = null;

            // script
            if (parametes.Length == 1 && parametes[0].ParameterType == typeof(string))
            {
                arguments = new object[] { script };
            }
            // arguments
            if (parametes.Length == 1 && parametes[0].ParameterType == typeof(object[]))
            {
                arguments = args;
            }
            // script & arguments
            if (parametes.Length == 2
                && parametes[0].ParameterType == typeof(string)
                && parametes[1].ParameterType == typeof(object[]))
            {
                arguments = new object[] { script, args };
            }

            // result
            return arguments;
        }

        /// <summary>
        /// Finds the first OpenQA.Selenium.IWebElement using the given method.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching OpenQA.Selenium.IWebElement on the current context.</returns>
        public IWebElement FindElement(By by) => MockWebElement.GetElement(this, by);

        /// <summary>
        /// Finds all OpenQA.Selenium.IWebElement within the current context using the given
        /// mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns> A collections of all <see cref="IWebElement"/> matching the current criteria, or an empty list if nothing matches.</returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            try
            {
                return MockWebElement.GetElements(this, by);
            }
            catch (Exception e) when (e is NoSuchElementException)
            {
                return new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
            }
        }

        /// <summary>
        /// Instructs the driver to change its settings.
        /// </summary>
        /// <returns> An <see cref="IOptions"/> object allowing the user to change the settings of the driver.</returns>
        public IOptions Manage() => new MockOptions()
        {
            Window = new MockWindow(new Point(x: xPosition, y: yPosition))
        };

        /// <summary>
        /// Instructs the driver to navigate the browser to another location.
        /// </summary>
        /// <returns>An <see cref="INavigation" /> object allowing the user to access the browser's history and to navigate to a given URL.</returns>
        public INavigation Navigate() => new MockNavigation();

        /// <summary>
        /// Quits this driver, closing every associated window.
        /// </summary>
        public void Quit()
        {
            // mock method - should not do anything
        }

        /// <summary>
        /// Instructs the driver to send future commands to a different frame or window.
        /// </summary>
        /// <returns>An <see cref="ITargetLocator" /> object which can be used to select a frame or window.</returns>
        public ITargetLocator SwitchTo() => new MockTargetLocator(this);

        /// <summary>
        /// Performs the specified list of actions with this action executor.
        /// </summary>
        /// <param name="actionSequenceList">The list of action sequences to perform.</param>
        public void PerformActions(IList<ActionSequence> actionSequenceList)
        {
            // mock method - should not do anything
        }

        /// <summary>
        /// Resets the input state of the action executor.
        /// </summary>
        public void ResetInputState()
        {
            // mock method - should not do anything
        }

        /// <summary>
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen.
        /// </summary>
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
        public Screenshot GetScreenshot() => new MockScreenshot(string.Empty);
        #endregion

        // sets child windows based on provided capabilities
        private void SetChildWindows(IDictionary<string, object> capabilities)
        {
            // normalize number of child windows
            int windows = 0;
            if (capabilities.ContainsKey(MockCapabilities.ChildWindows))
            {
                int.TryParse($"{capabilities[MockCapabilities.ChildWindows]}", out windows);
            }

            // setup window handles
            var windowHandles = new List<string> { $"window-{Guid.NewGuid()}" };

            // add handles for capabilities
            for (int i = 0; i < windows; i++)
            {
                windowHandles.Add($"window-{Guid.NewGuid()}");
            }

            // return a new collection with all handles
            WindowHandles = new ReadOnlyCollection<string>(windowHandles);
        }

        // EXECUTE SCRIPT FACTORY
#pragma warning disable S3400, IDE0051
        [Description("outerHTML")]
        private string SrcOuterHtml() =>
            "<div mock-attribute=\"mock attribute value\">mock element inner-text" +
            "   <div mock-attribute=\"mock attribute value\">mock element nested inner-text</div>" +
            "   <positive mock-attribute=\"mock attribute value\">mock element nested inner-text</positive>" +
            "   <positive mock-attribute=\"mock attribute value\">mock element nested inner-text</positive>" +
            "   <positive mock-attribute=\"mock attribute value\">mock element nested inner-text</positive>" +
            "</div>";

        [Description("scriptMacro")]
        private string SrcMarco() => "some text and number 777";

        [Description("readyState")]
        private string SrcReadyState()
        {
            // setup
            var random = new Random().Next(0, 100);

            // script result
            if (random < 20)
            {
                return "uninitialized";
            }
            if(random > 20 && random < 40)
            {
                return "loading";
            }
            if(random > 40 && random < 60)
            {
                return "loaded";
            }
            if(random > 60 && random < 80)
            {
                return "interactive";
            }
            return "complete";
        }

        [Description(".*invalid.*")]
        private string SrcInvalid() => throw new WebDriverException();

        [Description("^$|unitTesting|arguments\\[\\d+\\]\\.(?!scroll)(?!(.*invalid.*))|^document.forms.*.submit.*.;$")]
        private string SrcEmpty() => string.Empty;

        [Description(@"^window\.scroll")]
        private string SrcScroll(string script)
        {
            // setup
            var x = Regex.Match(script, @"(?<=top:(\s+)?)\d+").Value;
            var y = Regex.Match(script, @"(?<=left:(\s+)?)\d+").Value;

            // set state
            int.TryParse(x, out xPosition);
            int.TryParse(y, out yPosition);

            // result
            return string.Empty;
        }

        [Description(@"^arguments\[0]\.scroll")]
        private string SrcScrollElement(string script, object[] args)
        {
            // setup
            var x = Regex.Match(script, @"(?<=top:(\s+)?)\d+").Value;
            var y = Regex.Match(script, @"(?<=left:(\s+)?)\d+").Value;

            // element
            if (args?.Length > 0 && args[0].GetType() == typeof(MockWebElement))
            {
                ((MockWebElement)args[0]).Attributes = new Dictionary<string, string>
                {
                    ["scrollTop"] = y,
                    ["scrollLeft"] = x
                };
            }

            // result
            return string.Empty;
        }

        [Description(@"^return arguments\[0]\.scroll(Top|Left);$")]
        private string SrcGetScroll(string script, object[] args)
        {
            // setup
            var x = "0";
            var y = "0";

            // element
            if (args?.Length > 0 && args[0].GetType() == typeof(MockWebElement))
            {
                x = ((MockWebElement)args[0]).GetAttribute("scrollLeft");
                y = ((MockWebElement)args[0]).GetAttribute("scrollTop");
            }

            // result
            return script.Contains("scrollLeft") ? x : y;
        }
#pragma warning restore
    }
}