using System;
using System.Collections.Generic;
using System.Windows;

namespace PrintDialogX
{
    /// <summary>
    /// Represents a document that can be used by a <see cref="PrintDialog.PrintDialog"/> instance.
    /// </summary>
    public class PrintDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDocument"/> class.
        /// </summary>
        public PrintDocument()
        {
            Pages = new List<PrintPage>();
        }

        /// <summary>
        /// Gets or sets the name of the document, which will be visible in the print queue of the printer.
        /// </summary>
        public string DocumentName { get; set; } = "Untitled Document";

        /// <summary>
        /// Gets or sets the size of the document in pixels.
        /// </summary>
        public Size DocumentSize { get; set; } = new Size(8.5 * 96, 11 * 96);

        /// <summary>
        /// Gets or sets the default margin of the document in pixels.
        /// </summary>
        public double DocumentMargin
        {
            get
            {
                return _documentMargin;
            }
            set
            {
                if (value < 0)
                {
                    throw new PrintDocumentException("The value has to be greater than zero.");
                }
                else
                {
                    _documentMargin = value;
                }
            }
        }
        private double _documentMargin = 60;

        /// <summary>
        /// Gets or sets the collection of pages within the document.
        /// </summary>
        public ICollection<PrintPage> Pages { get; set; }

        /// <summary>
        /// Sets the <see cref="DocumentSize"/> property with values in inches.
        /// <param name="width">The width in inches.</param>
        /// <param name="height">The height in inches.</param>
        /// <param name="dpi">The DPI to be used for conversion calculation. The default value is 96.</param>
        /// </summary>
        /// <returns>The current document with the <see cref="DocumentSize"/> property updated.</returns>
        public PrintDocument SetSizeByInch(double width, double height, double dpi = 96)
        {
            DocumentSize = new Size(width * dpi, height * dpi);
            return this;
        }

        /// <summary>
        /// Creates an instance of the <see cref="PrintDocument"/> class from a <see cref="System.Windows.Documents.FixedDocument"/> object.
        /// <param name="document">The <see cref="System.Windows.Documents.FixedDocument"/> object.</param>
        /// <param name="documentName">The name of the document, which will be visible in the print queue of the printer.</param>
        /// <param name="documentMargin">The default margin of the document in pixels.</param>
        /// </summary>
        /// <returns>The created <see cref="PrintDocument"/> instance.</returns>
        public static PrintDocument CreateFromFixedDocument(System.Windows.Documents.FixedDocument document, string documentName, double documentMargin)
        {
            PrintDocument printDocument = new PrintDocument() { DocumentName = documentName, DocumentMargin = documentMargin, DocumentSize = document.DocumentPaginator.PageSize };
            foreach (System.Windows.Documents.PageContent pageContent in document.Pages)
            {
                System.Windows.Documents.FixedPage page = pageContent.Child;
                pageContent.Child = null;
                printDocument.Pages.Add(new PrintPage() { Content = page });
            }
            return printDocument;
        }
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
        public FrameworkElement Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (System.Windows.Media.VisualTreeHelper.GetParent(value) != null)
                {
                    throw new PrintDocumentException("The value is already the child of another element.");
                }
                else
                {
                    _content = value;
                }
            }
        }
        private FrameworkElement _content;
    }

    /// <summary>
    /// Represents errors that occur during using a <see cref="PrintDocument"/> instance.
    /// </summary>
    public class PrintDocumentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDocumentException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PrintDocumentException(string message) : base(message) { }
    }
}

