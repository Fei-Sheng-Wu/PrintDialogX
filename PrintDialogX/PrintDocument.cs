using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace PrintDialogX
{
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
        public double DocumentMargin
        {
            get => margin;
            set => margin = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(DocumentMargin), "The value cannot be negative.");
        }
        private double margin = 60;

        /// <summary>
        /// Gets or sets the collection of pages in the document.
        /// </summary>
        public ICollection<PrintPage> Pages { get; set; } = [];

        /// <summary>
        /// Gets the number of pages in the document.
        /// </summary>
        public int PageCount { get => Pages.Count; }

        /// <summary>
        /// Gets or sets the computed size of the available space for the content of the document, excluding the margin.
        /// </summary>
        public Size MeasuredSize { get; set; } = Size.Empty;

        /// <summary>
        /// Raises the <see cref="PrintSettingsChanged"/> event.
        /// </summary>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> instance to be used to invoke the handler.</param>
        /// <param name="settings">The <see cref="PrintSettingsEventArgs"/> instance of the new print settings.</param>
        public void OnPrintSettingsChanged(Dispatcher dispatcher, PrintSettingsEventArgs settings)
        {
            dispatcher.Invoke(() => PrintSettingsChanged?.Invoke(this, settings));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintPage"/> class.
    /// </summary>
    public class PrintPage
    {
        /// <summary>
        /// Gets or sets the content of the page.
        /// </summary>
        public FrameworkElement? Content
        {
            get => content;
            set => content = value == null || VisualTreeHelper.GetParent(value) == null ? value : throw new PrintDocumentException(value, "The value is already the child of another element.");
        }
        private FrameworkElement? content;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintDocumentException"/> class.
    /// </summary>
    /// <param name="content">The <see cref="FrameworkElement"/> instance that caused the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public class PrintDocumentException(FrameworkElement content, string message) : Exception(message)
    {
        /// <summary>
        /// Gets or sets the <see cref="FrameworkElement"/> instance that caused the error.
        /// </summary>
        public FrameworkElement Content { get; set; } = content;
    }
}

