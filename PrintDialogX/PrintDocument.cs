using System;
using System.Collections.Generic;
using System.Windows;

namespace PrintDialogX
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrintPage"/> class.
    /// </summary>
    public class PrintPage
    {
        /// <summary>
        /// Gets or sets the content of the page.
        /// </summary>
        /// <exception cref="PrintDocumentException">The value assigned is already the child of another element.</exception>
        public FrameworkElement? Content
        {
            get;
            set => field = value?.Parent is not DependencyObject parent ? value : throw new PrintDocumentException(value, "The value is already the child of another element.", parent);
        } = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintDocument"/> class.
    /// </summary>
    public class PrintDocument()
    {
        /// <summary>
        /// Occurs when the print settings have changed.
        /// </summary>
        public event EventHandler<PrintSettingsEventArgs>? PrintSettingsChanged = null;

        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        public string DocumentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the size of the document. If set to <see langword="null"/>, the document automatically adapts to the sizes calculated from the print settings.
        /// </summary>
        public Enums.Size? DocumentSize { get; set; } = null;

        /// <summary>
        /// Gets or sets the default margin of the document in pixels.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value assigned is not positive or zero.</exception>
        public double DocumentMargin
        {
            get;
            set => field = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(DocumentMargin), "The value must be positive or zero.");
        } = 60;

        /// <summary>
        /// Gets or sets the collection of pages in the document.
        /// </summary>
        public ICollection<PrintPage> Pages { get; set; } = [];

        /// <summary>
        /// Gets the number of pages in the document.
        /// </summary>
        public int PageCount { get => Pages.Count; }

        /// <summary>
        /// Gets the computed size of the available space for the content of the document, excluding the margin.
        /// </summary>
        public Size? MeasuredSize { get => measurement; }

        private Size? measurement = null;

        internal void UpdateMeasurement(Size size)
        {
            measurement = size;
        }

        internal void OnPrintSettingsChanged(PrintSettingsEventArgs settings)
        {
            PrintSettingsChanged?.Invoke(this, settings);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintDocumentException"/> class.
    /// </summary>
    /// <param name="content">The <see cref="FrameworkElement"/> instance that caused the error.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="context">The related context that is associated with the error.</param>
    public class PrintDocumentException(FrameworkElement content, string message, object? context = null) : Exception(message)
    {
        /// <summary>
        /// Gets or sets the <see cref="FrameworkElement"/> instance that caused the error.
        /// </summary>
        public FrameworkElement Content { get; } = content;

        /// <summary>
        /// Gets or sets the related context that is associated with the error.
        /// </summary>
        public object? Context { get; } = context;
    }
}
