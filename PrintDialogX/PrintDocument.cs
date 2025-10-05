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
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDocument"/> class.
        /// </summary>
        public PrintDocument() { }

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
            get => documentMargin;
            set => documentMargin = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(DocumentMargin), "The value cannot be negative.");
        }
        private double documentMargin = 60;

        /// <summary>
        /// Gets or sets the collection of pages within the document.
        /// </summary>
        public ICollection<PrintPage> Pages { get; set; } = [];

        public int PageCount
        {
            get => Pages.Count;
        }

        //TODO: document refresh event listener
        public event EventHandler<PrintSettings>? OnPrintSettingsChanged = null;
    }

    /// <summary>
    /// Represents a single page that can be included in a <see cref="PrintDocument"/> instance.
    /// </summary>
    public class PrintPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintPage"/> class.
        /// </summary>
        public PrintPage() { }

        /// <summary>
        /// Gets or sets the content of the page.
        /// </summary>
        public FrameworkElement? Content
        {
            get => content;
            set
            {
                if (value != null && VisualTreeHelper.GetParent(value) != null)
                {
                    throw new PrintDocumentException(value, "The value is already the child of another element.");
                }

                content = value;
            }
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

