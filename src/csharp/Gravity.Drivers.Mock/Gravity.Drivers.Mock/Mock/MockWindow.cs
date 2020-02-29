using System.Drawing;

namespace OpenQA.Selenium.Mock
{
    /// <summary>
    /// Provides methods for getting and setting the size and position of the browser
    /// window.
    /// </summary>
    /// <seealso cref="IWindow" />
    public class MockWindow : IWindow
    {
        #region *** constructors ***
        /// <summary>
        /// Initializes a new instance of the <see cref="MockWindow"/> class.
        /// </summary>
        public MockWindow()
            : this(position: new Point(1, 1), size: new Size(300, 300))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWindow"/> class.
        /// </summary>
        /// <param name="position">The position of the browser window relative to the upper-left corner of the screen.</param>
        public MockWindow(Point position)
            : this(position, size: new Size(300, 300))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWindow"/> class.
        /// </summary>
        /// <param name="position">The position of the browser window relative to the upper-left corner of the screen.</param>
        /// <param name="size">The outer browser window, including title bars and window borders.</param>
        public MockWindow(Point position, Size size)
        {
            Position = position;
            Size = size;
        }
        #endregion

        /// <summary>
        /// Gets or sets the position of the browser window relative to the upper-left corner of the screen.
        /// </summary>
        /// <remarks>
        /// When setting this property, it should act as the JavaScript window.moveTo() method.
        /// </remarks>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the size of the outer browser window, including title bars and window borders.
        /// </summary>
        /// <remarks>
        /// When setting this property, it should act as the JavaScript window.resizeTo() method.
        /// </remarks>
        public Size Size { get; set; }

        /// <summary>
        /// Sets the current window to full screen if it is not already in that state.
        /// </summary>
        public void FullScreen()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Maximizes the current window if it is not already maximized.
        /// </summary>
        public void Maximize()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Minimizes the current window if it is not already minimized.
        /// </summary>
        public void Minimize()
        {
            // Method intentionally left empty.
        }
    }
}