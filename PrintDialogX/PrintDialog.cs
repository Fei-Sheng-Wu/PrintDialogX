using System;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;

namespace PrintDialogX
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrintDialog"/> class.
    /// </summary>
    /// <param name="host">The custom <see cref="IPrintDialogHost"/> instance to be used to host the actual control for the print operation.</param>
    public class PrintDialog(IPrintDialogHost host)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialog"/> class.
        /// </summary>
        public PrintDialog() : this(new PrintDialogWindow()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialog"/> class.
        /// </summary>
        /// <param name="callback">The callback function that is invoked to customize the default <see cref="IPrintDialogHost"/> instance, which derives from <see cref="Window"/>.</param>
        public PrintDialog(Action<Window> callback) : this()
        {
            callback((Window)Host);
        }

        /// <summary>
        /// Gets or sets the <see cref="IPrintDialogHost"/> instance to host the actual control for the print operation.
        /// </summary>
        public IPrintDialogHost Host { get; set; } = host;

        /// <summary>
        /// Gets or sets the document to be printed.
        /// </summary>
        public PrintDocument? Document { get; set; } = null;

        /// <summary>
        /// Gets ot sets the <see cref="System.Printing.PrintServer"/> instance to be used to find printers. If set to <see langword="null"/>, the default <see cref="System.Printing.PrintServer"/> is used.
        /// </summary>
        public PrintServer? PrintServer { get; set; } = null;

        /// <summary>
        /// Gets or sets the default printer.
        /// </summary>
        public PrintQueue? DefaultPrinter { get; set; } = null;

        /// <summary>
        /// Gets or sets the default print settings.
        /// </summary>
        public PrintSettings PrintSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets the interface settings.
        /// </summary>
        public InterfaceSettings InterfaceSettings { get; set; } = new();

        /// <summary>
        /// Gets the result of the print operation.
        /// </summary>
        public PrintDialogResult Result { get => Host.GetResult(); }

        /// <summary>
        /// Opens the dialog.
        /// </summary>
        public void Show()
        {
            Host.Start(this, false, GetCallback());
        }

        /// <summary>
        /// Opens the dialog.
        /// </summary>
        /// <param name="generator">The callback function that is invoked asynchronously to generate the document while a spinner is displayed in the dialog.</param>
        public void Show(Func<Task> generator)
        {
            Host.Start(this, false, GetCallback(generator));
        }

        /// <summary>
        /// Opens the dialog and returns only when the dialog is closed.
        /// </summary>
        /// <returns><see langword="true"/> if the document was successfully printed; otherwise, <see langword="false"/>.</returns>
        public bool ShowDialog()
        {
            Host.Start(this, true, GetCallback());

            return Host.GetResult().IsSuccess;
        }

        /// <summary>
        /// Opens the dialog and returns only when the dialog is closed.
        /// </summary>
        /// <param name="generator">The callback function that is invoked asynchronously to generate the document while a spinner is displayed in the dialog.</param>
        /// <returns><see langword="true"/> if the document was successfully printed; otherwise, <see langword="false"/>.</returns>
        public bool ShowDialog(Func<Task> generator)
        {
            Host.Start(this, true, GetCallback(generator));

            return Host.GetResult().IsSuccess;
        }

        private Func<Task<FrameworkElement>> GetCallback(Func<Task>? generator = null)
        {
            return async () =>
            {
                if (generator != null)
                {
                    await generator();
                }

                return new PrintDialogControl(this, Host);
            };
        }
    }
}
