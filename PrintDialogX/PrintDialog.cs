using System;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;

namespace PrintDialogX
{
    /// <summary>
    /// Provides a custom print dialog with preview in real-time and powerful options.
    /// </summary>
    public class PrintDialog(IPrintDialogHost host)
    {
        public PrintDialog() : this(new PrintDialogWindow()) { }

        public PrintDialog(Action<Window> callback) : this()
        {
            callback((Window)Host);
        }

        public IPrintDialogHost Host { get; set; } = host;

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
        public PrintSettings PrintSettings { get; set; } = new();

        public InterfaceSettings InterfaceSettings { get; set; } = new();

        public PrintDialogResult Result { get => Host.GetResult(); }

        //TODO: documentation
        public void Show()
        {
            Host.Start(this, false, () => Task.FromResult<FrameworkElement>(new PrintDialogControl(this, Host)));
        }

        public void Show(Func<Task> generation)
        {
            Host.Start(this, false, async () =>
            {
                await generation();
                return new PrintDialogControl(this, Host);
            });
        }

        /// <summary>
        /// Opens the dialog and returns only when the dialog is closed.
        /// </summary>
        /// <returns><see langword="true"/> if the "Print" button was clicked; otherwise, <see langword="false"/>.</returns>
        public bool ShowDialog()
        {
            Host.Start(this, true, () => Task.FromResult<FrameworkElement>(new PrintDialogControl(this, Host)));

            return Host.GetResult().IsSuccess;
        }

        /// <summary>
        /// Opens the dialog with a synchronized document generation function and returns only when the dialog is closed.
        /// </summary>
        /// <param name="generation">The function that will be invoked to synchronously generate the document while the dialog is openning. The <see cref="Document"/> property must be set before the function completes.</param>
        /// <returns><see langword="true"/> if the "Print" button was clicked; otherwise, <see langword="false"/>.</returns>
        public bool ShowDialog(Func<Task> generation)
        {
            Host.Start(this, true, async () =>
            {
                await generation();
                return new PrintDialogControl(this, Host);
            });

            return Host.GetResult().IsSuccess;
        }
    }
}
