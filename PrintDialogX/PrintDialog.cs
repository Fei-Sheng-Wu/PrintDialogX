using System;
using System.Collections.Generic;
using System.Printing;
using System.Windows;

namespace PrintDialogX.PrintDialog
{
    /// <summary>
    /// Provides a custom print dialog with preview in real-time and powerful options.
    /// </summary>
    public class PrintDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialog"/> class.
        /// </summary>
        public PrintDialog() { }

        /// <summary>
        /// Gets or sets the <see cref="Window"/> that owns the dialog.
        /// </summary>
        public Window Owner { get; set; } = null;

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        public string Title { get; set; } = "Print";

        /// <summary>
        /// Gets or sets the icon of the dialog, or to use the default icon if the value is <see langword="null"/>.
        /// </summary>
        public System.Windows.Media.ImageSource Icon { get; set; } = null;

        /// <summary>
        /// Gets or sets whether the dialog appears in the topmost z-order.
        /// </summary>
        public bool Topmost { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the dialog has a task bar button.
        /// </summary>
        public bool ShowInTaskbar { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the dialog offers the "Pages" option in print settings, which contains "All Pages", "Current Page", and "Custom Pages".
        /// </summary>
        public bool AllowPagesOption { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the dialog offers the "Pages Per Sheet" option in print settings.
        /// </summary>
        public bool AllowPagesPerSheetOption { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the dialog offers the "Page Order" option in print settings.
        /// </summary>
        public bool AllowPageOrderOption { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the dialog offers the "Scale" option in print settings.
        /// </summary>
        public bool AllowScaleOption { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the dialog offers the "Double-Sided" option in print settings.
        /// </summary>
        public bool AllowDoubleSidedOption { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the dialog offers the "Add New Printer" button in the printer list.
        /// </summary>
        public bool AllowAddNewPrinterButton { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the dialog offers the "Printer Preferences" button.
        /// </summary>
        public bool AllowPrinterPreferencesButton { get; set; } = true;

        /// <summary>
        /// Gets or sets the document that needs to be printed.
        /// </summary>
        public PrintDocument Document { get; set; } = null;

        /// <summary>
        /// Gets or sets the resize mode of the dialog.
        /// </summary>
        public ResizeMode ResizeMode { get; set; } = ResizeMode.NoResize;

        /// <summary>
        /// Gets or sets the printer that should be initially selected. If it isn't found in the local print queue, then the local print queue's default is used instead.
        /// </summary>
        public PrintQueue InitialPrintQueue { get; set; } = null;

        /// <summary>
        /// Gets or sets the default print settings to be used.
        /// </summary>
        public PrintDialogSettings DefaultSettings { get; set; } = new PrintDialogSettings();

        /// <summary>
        /// Gets or sets the position of the dialog when first shown.
        /// </summary>
        public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.CenterScreen;

        /// <summary>
        /// Gets or sets the optional function that will be invoked to update the content of the document with the specific print settings set in the dialog. The function is required to return a collection of updated <see cref="PrintPage"/> in the original length and order.
        /// The function has no need to alter the content to resolve basic changes in print settings, such as to resize the pages based on the <see cref="PrintSettings.PageSize"/> property, as the dialog will take care of them.
        /// The function is intended to offer the ability to dynamically update the document for more complicated scenarios, such as styling asjuments or alternative formats for different media.
        /// </summary>
        public Func<DocumentInfo, ICollection<PrintPage>> ReloadDocumentCallback { get; set; } = null;

        /// <summary>
        /// Gets the total number of papers that the printer is calculated to be using.
        /// </summary>
        public int TotalPapers
        {
            get
            {
                return _totalPapers;
            }
        }
        private int _totalPapers = 0;

        /// <summary>
        /// Opens the dialog and returns only when the dialog is closed.
        /// </summary>
        /// <returns><see langword="true"/> if the "Print" button was clicked; otherwise, <see langword="false"/>.</returns>
        public bool ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Opens the dialog with a synchronized document generation function and returns only when the dialog is closed.
        /// </summary>
        /// <param name="documentGeneration">The function that will be invoked to synchronously generate the document while the dialog is openning. The <see cref="Document"/> property must be set before the function completes.</param>
        /// <returns><see langword="true"/> if the "Print" button was clicked; otherwise, <see langword="false"/>.</returns>
        public bool ShowDialog(Action documentGeneration)
        {
            if (documentGeneration == null && Document == null)
            {
                throw new ArgumentNullException("Document is null.");
            }

            Internal.PrintWindow dialog = new Internal.PrintWindow(this, documentGeneration)
            {
                Title = Title,
                Owner = Owner,
                Topmost = Topmost,
                ResizeMode = ResizeMode,
                ShowInTaskbar = ShowInTaskbar,
                WindowStartupLocation = WindowStartupLocation
            };
            dialog.Icon = Icon ?? dialog.Icon;
            dialog.ShowDialog();
            _totalPapers = dialog.TotalPapers;
            return dialog.ReturnValue;
        }
    }

    /// <summary>
    /// Defines the print settings that can be used by a <see cref="PrintDialog"/> instance.
    /// </summary>
    public class PrintDialogSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialogSettings"/> class.
        /// </summary>
        public PrintDialogSettings()
        {
            UsePrinterDefaultSettings = false;
        }

        /// <summary>
        /// Gets whether the default settings of printers are used. This will override all other settings.
        /// </summary>
        public bool UsePrinterDefaultSettings { get; internal set; } = false;

        /// <summary>
        /// Gets or sets the value indicating how the page content is oriented for printing.
        /// </summary>
        public PrintSettings.PageOrientation Layout { get; set; } = PrintSettings.PageOrientation.Portrait;

        /// <summary>
        /// Gets or sets the value indicating how the printer handles content that has color or shades of gray.
        /// </summary>
        public PrintSettings.PageColor Color { get; set; } = PrintSettings.PageColor.Color;

        /// <summary>
        /// Gets or sets the value indicating the quality of output for printing.
        /// </summary>
        public PrintSettings.PageQuality Quality { get; set; } = PrintSettings.PageQuality.Normal;

        /// <summary>
        /// Gets or sets the number of pages that print on each printed side of a sheet of paper.
        /// </summary>
        public PrintSettings.PagesPerSheet PagesPerSheet { get; set; } = PrintSettings.PagesPerSheet.One;

        /// <summary>
        /// Gets or sets the value indicating how a printer arranges multiple pages that print on each side of a sheet of paper.
        /// </summary>
        public PrintSettings.PageOrder PageOrder { get; set; } = PrintSettings.PageOrder.Horizontal;

        /// <summary>
        /// Gets or sets the value indicating what kind of two-sided printing, if any, that the printer uses for printing.
        /// </summary>
        public PrintSettings.DoubleSided DoubleSided { get; set; } = PrintSettings.DoubleSided.OneSided;

        /// <summary>
        /// Gets or sets the page size for the paper or other print media that the printer uses for printing.
        /// </summary>
        public PrintSettings.PageSize PageSize { get; set; } = PrintSettings.PageSize.NorthAmericaLetter;

        /// <summary>
        /// Gets or sets the value indicating the type of printing paper or other media that the printer uses for printing.
        /// </summary>
        public PrintSettings.PageType PageType { get; set; } = PrintSettings.PageType.Plain;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialogSettings"/> class that uses the default settings of printers.
        /// </summary>
        /// <returns>The created <see cref="PrintDialogSettings"/> instance.</returns>
        public static PrintDialogSettings PrinterDefaultSettings()
        {
            return new PrintDialogSettings() { UsePrinterDefaultSettings = true };
        }
    }

    /// <summary>
    /// Defines the print settings that is applied to a <see cref="PrintDocument"/> instance.
    /// </summary>
    public class DocumentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInfo"/> class.
        /// </summary>
        public DocumentInfo(int[] pages, PrintSettings.PageOrientation orientation, PrintSettings.PageColor color, PrintSettings.PagesPerSheet pagesPerSheet, PrintSettings.PageOrder pageOrder, double scale, double margin, Size size)
        {
            Pages = pages;
            Orientation = orientation;
            Color = color;
            PagesPerSheet = pagesPerSheet;
            PageOrder = pageOrder;
            Scale = scale;
            Margin = margin;
            Size = size;
        }

        /// <summary>
        /// Gets the array of 1-based indexes of each page of pages that are needed to be printed.
        /// </summary>
        public int[] Pages { get; }

        /// <summary>
        /// Gets the value indicating how the page content is oriented for printing.
        /// </summary>
        public PrintSettings.PageOrientation Orientation { get; }

        /// <summary>
        /// Gets the value indicating how the printer handles content that has color or shades of gray.
        /// </summary>
        public PrintSettings.PageColor Color { get; }

        /// <summary>
        /// Gets the number of pages that print on each printed side of a sheet of paper.
        /// </summary>
        public PrintSettings.PagesPerSheet PagesPerSheet { get; }

        /// <summary>
        /// Gets the value indicating how a printer arranges multiple pages that print on each side of a sheet of paper.
        /// </summary>
        public PrintSettings.PageOrder PageOrder { get; }

        /// <summary>
        /// Gets the percentage by which the printer enlarges or reduces the document on a page, or automatically scales to fit if the value is <see cref="double.NaN"/>.
        /// </summary>
        public double Scale { get; }

        /// <summary>
        /// Gets the distance between the printable area of the document and the nearest edge in pixels.
        /// </summary>
        public double Margin { get; }

        /// <summary>
        /// Gets the page size for the paper or other print media that the printer uses for printing in pixels without consideration of orientation.
        /// </summary>
        public Size Size { get; }
    }
}
