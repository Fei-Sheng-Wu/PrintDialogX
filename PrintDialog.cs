using System;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;

namespace PrintDialogX
{
    /// <summary>
    /// Provides a custom print dialog with preview in real-time and powerful options.
    /// </summary>
    public class PrintDialog
    {
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

        public Window PrintDialogWindow { get => window; }

        /// <summary>
        /// Gets the total number of papers that the printer is calculated to be using.
        /// </summary>
        public int TotalPaper { get => result != null ? result.Value.TotalPaper : 0; }

        private PrintDialogWindow window = new();
        private (bool IsSuccess, int TotalPaper)? result = null;

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
        public void Show()
        {
            Show(null);
        }

        public void Show(Func<Task>? generation)
        {
            StartWindow(generation);
            window.Show();
            window.Closed += (x, e) => EndWindow();
        }

        private void StartWindow(Func<Task>? generation = null)
        {
            if (PrintDialogWindow is not PrintDialogWindow window)
            {
                throw new InvalidOperationException("Unable to create the print dialog.");
            }

            window.Create(this, generation);
        }

        private bool EndWindow()
        {
            result = window.Result;
            window = new();

            return result != null && result.Value.IsSuccess;
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

        public string Title { get; set; } = (string)PrintDialogWindow.StringResources["StringResource_TitlePrint"];

        public Wpf.Ui.Controls.IconElement? Icon { get; set; } = new Wpf.Ui.Controls.SymbolIcon()
        {
            Symbol = Wpf.Ui.Controls.SymbolRegular.Print20,
            FontSize = 18
        };

        public Option[] BasicSettings { get; set; } = [Option.Printer, Option.PrinterPreferences, Option.Void, Option.Copies, Option.Collation, Option.Pages, Option.Layout, Option.Size];

        public Option[] AdvancedSettings { get; set; } = [Option.Color, Option.Quality, Option.PagesPerSheet, Option.PageOrder, Option.Scale, Option.Margin, Option.DoubleSided, Option.Type, Option.Source];
    }
}
