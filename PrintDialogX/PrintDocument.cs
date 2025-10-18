using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PrintDialogX
{
    /// <summary>
    /// Represents a document that can be used by a <see cref="PrintDialog"/> instance.
    /// </summary>
    public class PrintDocument
    {
        public event EventHandler<PrintSettings>? PrintSettingsChanged = null;

        /// <summary>
        /// Gets or sets the name of the document, which will be visible in the print queue of the printer.
        /// </summary>
        public string DocumentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the size of the document in pixels.
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
        /// Gets or sets the collection of pages within the document.
        /// </summary>
        public ICollection<PrintPage> Pages { get; set; } = [];

        public int PageCount { get => Pages.Count; }

        public void OnPrintSettingsChanged(PrintSettings settings)
        {
            PrintSettingsChanged?.Invoke(this, settings);
        }
    }

    /// <summary>
    /// Represents a single page that can be included in a <see cref="PrintDocument"/> instance.
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
    /// Represents errors that occur during using a <see cref="PrintDocument"/> instance.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public class PrintDocumentException(FrameworkElement content, string message) : Exception(message)
    {
        public FrameworkElement Content { get; set; } = content;
    }
}

