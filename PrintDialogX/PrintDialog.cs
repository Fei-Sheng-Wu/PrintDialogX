using System;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PrintDialogX
{
    /// <summary>
    /// Provides a custom print dialog with preview in real-time and powerful options.
    /// </summary>
    public class PrintDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialog"/> class.
        /// </summary>
        public PrintDialog()
        {
            try
            {
                DefaultPrinter = LocalPrintServer.GetDefaultPrintQueue();
            }
            catch { }
        }

        /// <summary>
        /// Gets or sets the <see cref="Window"/> that owns the dialog.
        /// </summary>
        public Window? Owner { get; set; } = null;

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        public string Title { get; set; } = "Print";

        /// <summary>
        /// Gets or sets the icon of the dialog.
        /// </summary>
        public ImageSource? Icon { get; set; } = null;

        /// <summary>
        /// Gets or sets the document that needs to be printed.
        /// </summary>
        public PrintDocument? Document { get; set; } = null;

        public PrintServer? PrintServer { get; set; } = null;

        /// <summary>
        /// Gets or sets the default printer to be used.
        /// </summary>
        public PrintQueue? DefaultPrinter { get; set; } = null;

        /// <summary>
        /// Gets or sets the default print settings to be used.
        /// </summary>
        public PrintSettings DefaultSettings { get; set; } = new();

        public InterfaceSettings Interface { get; set; } = new();

        public Window PrintDialogWindow
        {
            get => window;
        }
        private PrintDialogWindow window = new();

        /// <summary>
        /// Gets the total number of papers that the printer is calculated to be using.
        /// </summary>
        public int TotalPapers
        {
            get
            {
                return totalPapers;
            }
        }
        private int totalPapers = 0;

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
        /// <param name="generation">The function that will be invoked to synchronously generate the document while the dialog is openning. The <see cref="Document"/> property must be set before the function completes.</param>
        /// <returns><see langword="true"/> if the "Print" button was clicked; otherwise, <see langword="false"/>.</returns>
        public bool ShowDialog(Func<Task>? generation)
        {
            StartWindow(generation);
            window.ShowDialog();

            return EndWindow();
        }

        //TODO: documentation
        public bool Show()
        {
            return Show(null);
        }

        public bool Show(Func<Task>? generation)
        {
            StartWindow(generation);
            window.Show();

            return EndWindow();
        }

        private void StartWindow(Func<Task>? generation = null)
        {
            if (PrintDialogWindow is not PrintDialogWindow window)
            {
                throw new InvalidOperationException("Unable to create the print dialog.");
            }

            window.Create(this, generation);
            window.Owner = Owner;
            window.Title = Title;
            window.Icon = Icon;
        }

        private bool EndWindow()
        {
            bool value = window.ReturnValue;
            totalPapers = window.TotalPapers;
            window = new();

            return value;
        }
    }

    public class InterfaceSettings
    {
        public enum Option
        {
            Void = -1,
            Printer,
            PrinterPreferences,
            Copies,
            Collation,
            Pages,
            Layout,
            Size,
            Color,
            Quality,
            PagesPerSheet,
            PageOrder,
            Scale,
            Margin,
            DoubleSided,
            Type,
            Source
        }

        public Option[] BasicSettings { get; set; } = [Option.Printer, Option.PrinterPreferences, Option.Void, Option.Copies, Option.Collation, Option.Pages, Option.Layout, Option.Size];

        public Option[] AdvancedSettings { get; set; } = [Option.Color, Option.Quality, Option.PagesPerSheet, Option.PageOrder, Option.Scale, Option.Margin, Option.DoubleSided, Option.Type, Option.Source];
    }
}
